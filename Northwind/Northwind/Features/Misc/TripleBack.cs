using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Queries;
using System;
using System.Linq;
using Raven.Client.Documents.Queries.Timings;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Misc
{
    public class TripleBack
    {
        public void Do()
        {
            var session = DocumentStoreHolder.Store.OpenSession();

            QueryTimings timings = new QueryTimings();

            var res = (from entry in session
                        .Query<Orders_ByProduct_BySupplier.Entry, Orders_ByProduct_BySupplier>()
                        .Customize(x => x.Timings(out timings))

                       where entry.Supplier == "suppliers/7-A"

                       let product = RavenQuery.Load<Product>(entry.Product)
                       let order = RavenQuery.Load<Order>(entry.Order) // ?

                       select new
                       {
                           Product = product,
                           Order = order
                       })
                
                .ToList();

            var x = res
                .GroupBy(x => x.Product.Id, 
                    (key, value) => new {
                        ProductId = key,
                        Product = value.ToList().Select(x => x.Product).ToList(),
                        Orders = value.ToList().Select(x => x.Order).ToList()
                    })
                .ToList();

            foreach (var entry in x)
            {
                Product product = entry.Product.First();
                Console.WriteLine($"[{product.Id}] {product.Name}");
                
                foreach (Order order in entry.Orders)
                    Console.WriteLine($"\t {order.Id}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
            Console.WriteLine($"Total execution time: {timings.DurationInMs}ms");
        }

        public void Do2()
        {
            var session = DocumentStoreHolder.Store.OpenSession();

            QueryTimings timings = new QueryTimings();

            var res = (from entry in session
                        .Query<Orders_ByProduct_BySupplier.Entry, Orders_ByProduct_BySupplier>()
                        .Customize(x => x.Timings(out timings))

                       where entry.Supplier == "suppliers/7-A"

                       let product = RavenQuery.Load<Product>(entry.Product)
                       let order = RavenQuery.Load<Order>(entry.Order) // ?

                       select new
                       {
                           Product = product,
                           Order = order
                       })
                
                .ToList();

            var x = res
                .GroupBy(x => x.Product.Id, 
                    (key, value) => new {
                        ProductId = key,
                        Product = value.ToList().Select(x => x.Product).ToList(),
                        Orders = value.ToList().Select(x => x.Order).ToList()
                    })
                .ToList();

            foreach (var entry in x)
            {
                Product product = entry.Product.First();
                Console.WriteLine($"[{product.Id}] {product.Name}");
                
                foreach (Order order in entry.Orders)
                    Console.WriteLine($"\t {order.Id}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
            Console.WriteLine($"Total execution time: {timings.DurationInMs}ms");
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
