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
            DocumentStore store = (DocumentStore)DocumentStoreHolder.GetStore();
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
            DocumentStore store = (DocumentStore)DocumentStoreHolder.GetStore();
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
            DocumentStore store = (DocumentStore)DocumentStoreHolder.GetStore();
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

        public void DocumentSessionIdentityMap()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            session.Load<Employee>("employees/8-A");
            session.Load<Employee>("employees/8-A");
            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void DocumentSessionEvict()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee emp = session.Load<Employee>("employees/8-A");
            session.Advanced.Evict(emp);
            session.Load<Employee>("employees/8-A");
            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void DocumentSessionNotModified()
        {
            DocumentStore store = (DocumentStore)DocumentStoreHolder.GetStore();
            store.OnSucceedRequest += (sender, e) => Console.WriteLine($"{e.Response.StatusCode}");
            store.Initialize();

            using (var s1 = store.OpenSession())
            {
                s1.Load<Employee>("employees/8-A");
            }

            using (var s2 = store.OpenSession())
            {
                s2.Load<Employee>("employees/8-A");
            }
        }
    }
}
