using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Queries;
using System;
using System.Linq;

namespace Northwind.Features.Misc
{
    public class TripleBack
    {
        public void Do()
        {
            var session = DocumentStoreHolder.Store.OpenSession();

            var res = (from entry in session.Query<Orders_ByProduct_BySupplier.Entry, Orders_ByProduct_BySupplier>()
                       where entry.Supplier == "suppliers/7-A"

                       let product = RavenQuery.Load<Product>(entry.Product)
                       let order = RavenQuery.Load<Order>(entry.Order) // ?

                       select new
                       {
                           Product = product.Name,
                           OrderId = order.Id,
                           Order = order
                       })
                      .ToList();

            foreach (var r in res)
            {
                Console.WriteLine($"{r.Product} \t {r.OrderId} \t {r.Order.Id}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }
    }

    public class Orders_ByProduct_BySupplier : AbstractIndexCreationTask<Order, Orders_ByProduct_BySupplier.Entry>
    {
        public class Entry
        {
            public string Order { get; set; }

            public string Product { get; set; }

            public string Supplier { get; set; }
        }

        public Orders_ByProduct_BySupplier()
        {
            Map = orders => from order in orders
                            from orderLine in order.Lines
                            let p = LoadDocument<Product>(orderLine.Product)
                            let s = LoadDocument<Supplier>(p.Supplier)
                            select new Entry
                            {
                                Order = order.Id,
                                Product = p.Id,
                                Supplier = s.Id
                            };

            Stores.Add(x => x.Product, FieldStorage.Yes);
            Stores.Add(x => x.Supplier, FieldStorage.Yes);
            Stores.Add(x => x.Order, FieldStorage.Yes);
        }
    }
}
