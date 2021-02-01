using System;
using System.Security.Cryptography.X509Certificates;
using Raven.Client.Documents;

namespace Northwind
{
    class Program
    {
        static void Main(string[] args)
        {
            using var store = new DocumentStore
            {
                Database = "demo",
                Certificate = new X509Certificate2("admin.client.certificate.d2.pfx"),
                Urls = new string[] {"https://a.d2.development.run/"}
            }.Initialize();
        }
    }
}
