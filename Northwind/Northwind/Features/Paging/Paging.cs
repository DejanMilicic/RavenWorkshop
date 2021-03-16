using System;
using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Paging
{
    public class Paging
    {
        public void FetchPage()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            int page = 1;
            int pageSize = 10;

            var pagedResults = session.Query<Order>()
                .Statistics(out QueryStatistics stats) //this is optional
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToList();

            int totalResults = stats.TotalResults;

            Console.WriteLine($"Showing orders {page * pageSize + 1} to {(page + 1) * pageSize} of {stats.TotalResults} total");
        }
    }
}
