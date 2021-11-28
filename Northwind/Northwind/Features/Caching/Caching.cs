using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            var store = DocumentStoreHolder.GetStore();
            store.OnSucceedRequest += (sender, e) => Console.WriteLine($"{e.Url} {e.Response.StatusCode}");
            store.Initialize();

            while (true)
            {
                using var session = store.OpenSession();
                session.Load<Employee>("employees/2-a");

                Console.ReadLine();
            }
        }

        public void QueryCaching()
        {
            var store = DocumentStoreHolder.GetStore();
            store.OnSucceedRequest += (sender, e) => Console.WriteLine($"{e.Url} {e.Response.StatusCode}");
            store.Initialize();

            while (true)
            {
                using var session = store.OpenSession();
                session.Query<Order>()
                    .Where(x => x.OrderedAt < DateTime.Today)
                    .ToList();

                Console.ReadLine();
            }
        }

        public void AggressiveCaching()
        {
            var store = DocumentStoreHolder.GetStore();
            store.OnSucceedRequest += (sender, e) => Console.WriteLine($"{e.Url} {e.Response.StatusCode}");
            store.Initialize();

            while (true)
            {
                using (store.OpenSession().Advanced.DocumentStore.AggressivelyCache())
                {
                    using var session = store.OpenSession();
                    var orders = session.Query<Order>()
                        .Where(x => x.OrderedAt < DateTime.Today)
                        .ToList();

                    Console.WriteLine(orders.Count);
                }

                Console.ReadLine();
            }
        }
    }
}
