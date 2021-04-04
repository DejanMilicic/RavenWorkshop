using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Client
{
    public class Client
    {
        public void Do()
        {
            using var session = Dsh.Store.OpenSession();

            var employees = session.Query<Employee>().Count();

            Console.WriteLine($"Total employees: {employees}");
        }
    }

    public static class Dsh
    {
        private static readonly Lazy<IDocumentStore> LazyStore =
            new Lazy<IDocumentStore>(() =>
            {
                var store = new DocumentStore
                {
                    Urls = new[]
                    {
                        "https://a.dejan.development.run/",
                        "https://b.dejan.development.run/",
                        "https://c.dejan.development.run/"
                    },
                    Certificate = new X509Certificate2(@"C:\temp\ravendb\admin.client.certificate.dejan.pfx"),
                    Database = "demo"
                };

                store.Initialize();

                IndexCreation.CreateIndexes(typeof(Program).Assembly, store);

                return store;
            });

        public static IDocumentStore Store => LazyStore.Value;
    }
}
