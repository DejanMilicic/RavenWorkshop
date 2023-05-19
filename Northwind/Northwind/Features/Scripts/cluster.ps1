Add-Type -AssemblyName System.Web

$count = 3

write-host "Starting nodes..."

for ($i=0; $i -lt $count; $i++) {
	 start "C:\wrkbasic\raven\Server\Raven.Server.exe" @("--ServerUrl=http://localhost:808$i", "--Security.UnsecuredAccessAllowed=PublicNetwork", "--Cluster.TcpTimeoutInMs=5000", "--Cluster.TimeBeforeAddingReplicaInSec=5", "--Cluster.TimeBeforeMovingToRehabInSec=5", "--DataDir=data/$i", "--Logs.Path=logs/$i", "--Setup.Mode=None", "--License.Eula.Accepted=true", "--License.Path=C:\wrkbasic\devlicense.txt")
}

Start-Sleep -Seconds 5 # let nodes start

write-host "Setting up a cluster..."

function DoCurl() {
    param($Method, $Uri)
    #write-host $Method
    #write-host $Uri
    Invoke-WebRequest -Method $Method -Uri $Uri -UseBasicParsing
}

function AddNodeToCluster() {
    param($FirstNodeUrl, $OtherNodeUrl, $AssignedCores = 1)

    $otherNodeUrlEncoded = [System.Web.HttpUtility]::UrlEncode($OtherNodeUrl)
    $uri = "$($FirstNodeUrl)/admin/cluster/node?assignedCores=$AssignedCores&url=$($otherNodeUrlEncoded)"
    DoCurl -Method 'PUT' -Uri $uri
    Start-Sleep -Seconds 5
}

$firstNodeUrl = "http://127.0.0.1:8080"

$coresReassigned = $false
function ReassignCoresOnFirstNode() {
    write-host "Reassign cores on $firstNodeUrl"
    $uri = "$firstNodeUrl/admin/license/set-limit?nodeTag=A&newAssignedCores=1"
    DoCurl -Method 'POST' -Uri $uri 
}

for ($i=1; $i -lt $count; $i++) {
    Write-Host "Add node $i to cluster";
    AddNodeToCluster -FirstNodeUrl $firstNodeUrl -OtherNodeUrl "http://127.0.0.1:808$i" -AssignedCores 1
    if ($coresReassigned -eq $False) {
        ReassignCoresOnFirstNode
        $coresReassigned = $true
    }
}