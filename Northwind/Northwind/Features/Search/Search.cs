using System;
using System.Linq;
using Northwind.Features.Indexes;
using Northwind.Models.Entity;
using Raven.Client.Documents;

namespace Northwind.Features.Search
{
    public class Search
    {
        public void SearchOrderLines()
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
    }
}
