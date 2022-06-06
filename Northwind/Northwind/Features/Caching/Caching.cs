using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using Raven.Client.Http;

namespace Northwind.Features.Caching
{
    public static class Caching
    {
        private static void DisplayResponseInfo(SucceedRequestEventArgs e)
        {
            Console.WriteLine("\nResponse");
            Console.WriteLine($"Status Code: {e.Response.StatusCode}");
            Console.WriteLine($"Payload size: {e.Response.Content.ReadAsByteArrayAsync().Result.Length} bytes");
        }

        public static void DocumentCaching()
        {
            DocumentStore store = (DocumentStore)DocumentStoreHolder.GetStore();
            store.OnSucceedRequest += (sender, e) => { DisplayResponseInfo(e); };
            store.Initialize();

            while (true)
            {
                using (var session = store.OpenSession())
                {
                    session.Load<Employee>("employees/2-a");
                }

                Console.ReadLine();
            }
        }

        public static void QueryCaching()
        {
            DocumentStore store = (DocumentStore)DocumentStoreHolder.GetStore();
            store.OnSucceedRequest += (sender, e) => { DisplayResponseInfo(e); };
            store.Initialize();

            while (true)
            {
                using (var session = store.OpenSession())
                {
                    session.Query<Order>()
                        .Where(x => x.OrderedAt < DateTime.Today)
                        .ToList();
                }

                Console.ReadLine();
            }
        }

        public static void AggressiveCaching()
        {
            DocumentStore store = (DocumentStore)DocumentStoreHolder.GetStore();
            store.OnSucceedRequest += (sender, e) => { DisplayResponseInfo(e); };
            store.Initialize();

            while (true)
            {
                using (var session = store.OpenSession())
                using (session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromSeconds(20)))
                {
                    var orders = session.Query<Order>()
                        .Where(x => x.OrderedAt < DateTime.Today)
                        .ToList();

                    Console.WriteLine($"Total number of orders: {orders.Count}");
                }

                Console.ReadLine();
            }
        }

        public static void AggressiveCachingTwoSessions()
        {
            DocumentStore store = (DocumentStore)DocumentStoreHolder.GetStore();
            store.OnSucceedRequest += (sender, e) => { DisplayResponseInfo(e); };
            store.Initialize();

            while (true)
            {
                // Session 1
                using (var session = store.OpenSession())
                using (session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromSeconds(10)))
                {
                    var orders = session.Query<Order>()
                        .Where(x => x.OrderedAt < DateTime.Today)
                        .ToList();

                    Console.WriteLine($"session #1 Total number of orders: {orders.Count}");
                }

                // Session 2
                using (var session = store.OpenSession())
                using (session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromSeconds(10)))
                {
                    var orders = session.Query<Order>()
                        .Where(x => x.OrderedAt < DateTime.Today)
                        .ToList();

                    Console.WriteLine($"session #2 Total number of orders: {orders.Count}");
                }

                Console.ReadLine();
            }
        }

        public static void AggressiveCachingNoChangeTracking()
        {
            DocumentStore store = (DocumentStore)DocumentStoreHolder.GetStore();
            store.OnSucceedRequest += (sender, e) => { DisplayResponseInfo(e); };
            store.Initialize();

            while (true)
            {
                using (var session = store.OpenSession())
                using (session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromSeconds(50), AggressiveCacheMode.DoNotTrackChanges))
                {
                    var orders = session.Query<Order>()
                        .Where(x => x.OrderedAt < DateTime.Today)
                        .ToList();

                    Console.WriteLine($"Total number of orders: {orders.Count}");
                }

                Console.ReadLine();
            }
        }

        public static void DocumentSessionIdentityMap()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            session.Load<Employee>("employees/8-A");
            session.Load<Employee>("employees/8-A");
            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public static void DocumentSessionEvict()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee emp = session.Load<Employee>("employees/8-A");
            session.Advanced.Evict(emp);
            session.Load<Employee>("employees/8-A");
            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public static void DocumentSessionNotModified()
        {
            DocumentStore store = (DocumentStore)DocumentStoreHolder.GetStore();
            store.OnSucceedRequest += (sender, e) => { DisplayResponseInfo(e); };
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
