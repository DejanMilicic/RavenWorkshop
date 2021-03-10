using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Misc
{
    public class Repro
    {
        public void Execute()
        {
            var store = new DocumentStore
            {
                Urls = new string[] { "http://live-test.ravendb.net/" },
                Database = "demo"
            }.Initialize();

            using var session = store.OpenSession();

            var projection = (from order in session.Query<Models.Entity.Order>()
                let company = RavenQuery.Load<Company>(order.Company)
                select new
                {
                    company
                }).ToList();
        }
    }

    public class Company
    {
        public string Id { get; set; } = "INITIAL";
        public string Name { get; set; } = "INITIAL";
    }
}
