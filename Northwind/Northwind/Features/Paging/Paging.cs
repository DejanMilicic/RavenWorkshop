using System;
using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Paging
{
    public static class Paging
    {
        public static void FetchPage()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            int page = 1;
            int pageSize = 10;


            var rql = session.Query<Order>()
                .Statistics(out QueryStatistics s1) // statistics are optional
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToString();

            Console.WriteLine(rql);
            Console.WriteLine();

            var pagedResults = session.Query<Order>()
                .Statistics(out QueryStatistics stats) // statistics are optional
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToList();

            long totalResults = stats.TotalResults;

            Console.WriteLine("OrderId \tFreight \tDestination");
            Console.WriteLine("--------\t--------\t-----------");

            foreach (Order order in pagedResults)
            {
                Console.WriteLine($"{order.Id} \t{order.Freight} lbs \t{order.ShipTo.City}");
            }
            
            Console.WriteLine();
            Console.WriteLine($"Showing orders {page * pageSize + 1} to {(page + 1) * pageSize} " +
                              $"of {stats.TotalResults} total in {stats.DurationInMs} ms");
        }
    }
}
