using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Misc
{
    public class ViewModel
    {

    }

    public class TripleBack
    {
        public void Do()
        {
            var session = DocumentStoreHolder.Store.OpenSession();

            var res = (from entry in session.Query<Orders_ByProduct_BySupplier.Entry, Orders_ByProduct_BySupplier>()
                      where entry.Supplier == "suppliers/7-A"
                      select new
                          {
                            Supplier = entry.Supplier,
                            Product = entry.Product,
                            Order = entry.Id
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
