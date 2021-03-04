using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features
{
    public partial class Examples
    {
        public readonly IDocumentStore store;

        public Examples()
        {
            store = new DocumentStore
            {
                Urls = new string[] { "https://a.free.dejanmilicic.ravendb.cloud/" },
                Certificate = new X509Certificate2("free.dejanmilicic.client.certificate.pfx"),
                Database = "demo"
            }.Initialize();

            IndexCreation.CreateIndexes(typeof(Program).Assembly, store);
        }
    }
}
