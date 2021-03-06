using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Features.Indexes;
using Northwind.Models.Entity;
using Raven.Client.Documents;

namespace Northwind.Features
{
    public partial class Examples
    {
        public void SearchOrderLines(string term)
        {
            using var session = store.OpenSession();

            var res = session.Query<Orders_Search.Entry, Orders_Search>()
                .Search(x => x.Query, term)
                .OfType<Order>()
                .ToList();

            foreach (Order order in res)
            {
                Console.WriteLine($"{order.Id}");
            }
        }
    }
}
