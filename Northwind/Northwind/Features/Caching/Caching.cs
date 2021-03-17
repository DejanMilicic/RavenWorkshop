using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Caching
{
    public class Caching
    {
        public void DocumentCaching()
        {
            while (true)
            {
                using var session = Dsh.Store.OpenSession();
                session.Load<Employee>("employees/2-a");

                Console.ReadLine();
            }
        }

        public void QueryCaching()
        {
            while (true)
            {
                using var session = Dsh.Store.OpenSession();
                session.Query<Order>()
                    .Where(x => x.OrderedAt < DateTime.Today)
                    .ToList();

                Console.ReadLine();
            }
        }
    }

    public static class Dsh
    {
        private static readonly Lazy<IDocumentStore> LazyStore =
            new Lazy<IDocumentStore>(() =>
            {
                var store = new DocumentStore
                {
                    Urls = new[] { "http://127.0.0.1:8080" },
                    Database = "demo"
                };

                store.OnSucceedRequest += (sender, e) =>
                {
                    Console.WriteLine($"{e.Url} {e.Response.StatusCode}");
                };

                store.Initialize();

                IndexCreation.CreateIndexes(typeof(Program).Assembly, store);

                return store;
            });

        public static IDocumentStore Store => LazyStore.Value;
    }
}
