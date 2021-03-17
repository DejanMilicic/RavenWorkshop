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

                       let supplier = RavenQuery.Load<Supplier>(entry.Supplier)
                       let product = RavenQuery.Load<Product>(entry.Product)
                       let order = RavenQuery.Load<Order>(entry.Id) // ?

                       select new
                       {
                           Supplier = supplier.Name,
                           Product = product.Name,
                           Order = entry.Id // order.Id ??
                       })
                      .ToList();

            foreach (var r in res)
            {
                Console.WriteLine($"{r.Supplier} -> {r.Product} -> {r.Order}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }
    }

    public class Orders_ByProduct_BySupplier : AbstractIndexCreationTask<Order, Orders_ByProduct_BySupplier.Entry>
    {
        public class Entry
        {
            public string Id { get; set; }

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
                                Product = p.Id,
                                Supplier = s.Id
                            };

            Stores.Add(x => x.Product, FieldStorage.Yes);
            Stores.Add(x => x.Supplier, FieldStorage.Yes);
        }
    }
}
