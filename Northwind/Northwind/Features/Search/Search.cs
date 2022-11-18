using System;
using System.Collections.Generic;
using System.Linq;
using Northwind.Features.Indexes;
using Northwind.Models.Entity;
using Raven.Client.Documents;

namespace Northwind.Features.Search
{
    public static class Search
    {
        public static void SearchOrderLines()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            string term = "apples";

            var res = session.Query<Orders_Search.Entry, Orders_Search>()
                .Search(x => x.Query, term)
                .OfType<Order>()
                .ToList();

            foreach (Order order in res)
            {
                Console.WriteLine($"Order {order.Id} contains {term}");
            }
        }

        public static void OrdersOmniSearch()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            string term = "maison"; // supplier name

            var res = session.Query<Orders_Omnisearch.Entry, Orders_Omnisearch>()
                .Search(x => x.Query, term)
                .OfType<Order>()
                .ToList();

            Console.WriteLine($"{res.Count} orders returned when searching for {term}");
            
            term = "apples"; // product name

            res = session.Query<Orders_Omnisearch.Entry, Orders_Omnisearch>()
                .Search(x => x.Query, term)
                .OfType<Order>()
                .ToList();

            Console.WriteLine($"{res.Count} orders returned when searching for {term}");            
            
            term = "boxes"; // packaging

            res = session.Query<Orders_Omnisearch.Entry, Orders_Omnisearch>()
                .Search(x => x.Query, term)
                .OfType<Order>()
                .ToList();

            Console.WriteLine($"{res.Count} orders returned when searching for {term}");
        }

        public static void Omnisearch()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            IList<Omnisearch.Projection> results = session
                .Query<Omnisearch.Entry, Omnisearch> ()
                .Search(x => x.Content, "Lau*")
                .ProjectInto<Omnisearch.Projection>()
                .ToList();

            foreach (Omnisearch.Projection result in results)
            {
                Console.WriteLine($"{result.Collection}: {result.DisplayName} [{result.Id}]");
                // Companies: Laughing Bacchus Wine Cellars
                // Products: Laughing Lumberjack Lager
                // Employees: Laura Callahan
            }
        }
    }
}
