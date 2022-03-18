using System;
using System.Security.Cryptography.X509Certificates;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Certificates
{
    // How to set up RavenDB cluster with your own certificate, Windows edition
    //
    // Note: all steps below should be executed with PowerShell console in Admin mode
    //
    // In the end, we will have cluster with three nodes:
    // a.ravendb.local
    // b.ravendb.local
    // c.ravendb.local
    //
    // 1. We need to set up local DNS for these three nodes
    // run notepad as Administrator
    // open c:\windows\system32\drivers\etc\hosts and add these three lines
    // 
    // 127.0.0.1 a.ravendb.local
    // 127.0.0.2 b.ravendb.local
    // 127.0.0.3 c.ravendb.local
    //
    // 2. You might need to flush DNS cache with
    //
    // > ipconfig /flushdns
    // 
    // 3. Generate a self-signed certificate
    //
    // > $selfSignedCert = New-SelfSignedCertificate -DnsName *.ravendb.local -NotAfter (Get-Date).AddYears(2)
    //
    // 4. Export the self-signed certificate into PFX format from Certificate manager
    // after this step, selfSignedCertificate.pfx file will be generated in the current folder
    //
    // > $pwd = ConvertTo-SecureString -String "1234" -Force -AsPlainText
    // > Export-PfxCertificate -cert $selfSignedCert.PSPath -FilePath "selfSignedCertificate.pfx" -Password $pwd
    //
    // 5. Import certificate to Trusted Root Certificate Authorities store
    // after this step Chrome will recognize your certificate as a trust one
    // and your code will be able to use it to access RavenDB cluster
    // without this step, your code will be rejected when attempting to connect
    //
    // > Import-PfxCertificate -FilePath "selfSignedCertificate.pfx" -Password $pwd -CertStoreLocation Cert:\LocalMachine\Root
    //
    // 6. Convert the certificate to Base64 encoding
    // after this step, selfSignedCertificate.txt file will be generated in the current folder
    // this file will contain base64-encoded content of the pfx certificate
    // later on, you can use it from your code to initialize DocumentStore
    //
    // > $pfxBytes = Get-Content "selfSignedCertificate.pfx" -Encoding Byte
    // > [System.Convert]::ToBase64String($pfxBytes) | Out-File "selfSignedCertificate.txt"
    //
    // This step is also applicable if you want to use pre-existing pfx file
    // which you did not generate, but you got it
    //
    // 7. Certificate, if not protected properly, can be single point of security breach in your system
    // you should take care how you handle it
    // In ideal case, you will not commit it under version control, send it via email, etc
    // Ideally, certificate will be delivered to your code in run time
    // Usual solution for this are various Secrets Managers
    //
    // For the purpose of this exercise, we will store certificate as a env variable of your Windows machine
    //
    // 8. Create environment variable RAVENDB_LOCAL_CERT that will contain BASE64-encoded certificate
    //
    // > $certText = Get-Content "selfSignedCertificate.txt"
    //
    // to create an environment variable visible to every process running on the machine:
    // > [System.Environment]::SetEnvironmentVariable('RAVENDB_LOCAL_CERT',$certText,[System.EnvironmentVariableTarget]::Machine)
    //
    // to create an environment variable which will be available to all processes that your account runs
    // > [System.Environment]::SetEnvironmentVariable('RAVENDB_LOCAL_CERT',$certText,[System.EnvironmentVariableTarget]::User)
    //
    // 9. Run cluster setup for RavenDB following steps from the documentation
    // https://ravendb.net/docs/article-page/latest/csharp/start/installation/setup-wizard#secure-setup-with-your-own-certificate

    public static class SelfSignedCert
    {
        public static void Do()
        {
            IDocumentStore store = new DocumentStore
            {
                Urls = new[]
                {
                    "https://a.ravendb.local",
                    "https://b.ravendb.local",
                    "https://c.ravendb.local"
                },
                Database = "test",
                Certificate = new X509Certificate2(Convert.FromBase64String(
                    Environment.GetEnvironmentVariable("RAVENDB_LOCAL_CERT")), // reading cert from env
                    "1234") // password you stated in step 4 when pfx file was created
            }.Initialize();

            // pay attention to https://snede.net/the-most-dangerous-constructor-in-net/

            using IDocumentSession session = store.OpenSession();

            session.Store(
                new Employee
                {
                    FirstName = "John",
                    LastName = "Doe"
                });
            session.SaveChanges();
        }
    }
}
