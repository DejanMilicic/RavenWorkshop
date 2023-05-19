$count = 3

for ($i=0; $i -lt $count; $i++) 
{
	 start "C:\wrkbasic\raven\Server\Raven.Server.exe" @("--ServerUrl=http://localhost:808$i", "--Security.UnsecuredAccessAllowed=PublicNetwork", "--Cluster.TcpTimeoutInMs=5000", "--Cluster.TimeBeforeAddingReplicaInSec=5", "--Cluster.TimeBeforeMovingToRehabInSec=5", "--DataDir=data/$i", "--Logs.Path=logs/$i", "--Setup.Mode=None", "--License.Eula.Accepted=true", "--License.Path=C:\wrkbasic\devlicense.txt")
}

