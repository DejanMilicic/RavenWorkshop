using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Queries.Timings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Northwind.Features.Misc
{
    public class TripleBack
    {
        public class Result
        {
            public void AddEntry(Supplier supplier, Product product, List<Order> orders)
            {
                //if orders.Any()

                var s = supplierEntries.FirstOrDefault(x => x.Supplier.Id == supplier.Id);
                if (s == null)
                {
                    s = new SupplierEntry();
                    s.Supplier = supplier;
                    supplierEntries.Add(s);
                }

                var p = s.ProductEntries.FirstOrDefault(x => x.Product.Id == product.Id);
                if (p == null)
                {
                    p = new SupplierEntry.ProductEntry();
                    p.Product = product;
                    s.ProductEntries.Add(p);
                }

                p.Orders.AddRange(orders);
            }

            public void Print()
            {
                foreach (var supplierEntry in supplierEntries)
                {
                    Console.WriteLine($"Supplier: {supplierEntry.Supplier.Id}");

                    foreach (var productEntry in supplierEntry.ProductEntries)
                    {
                        Console.WriteLine($"\t Product: {productEntry.Product.Id}");

                        foreach (var order in productEntry.Orders)
                        {
                            Console.WriteLine($"\t\t Order: {order.Id}");
                        }
                    }
                }

                Console.WriteLine();
                Console.WriteLine();
            }

            public List<SupplierEntry> supplierEntries { get; set; } = new List<SupplierEntry>();

            public class SupplierEntry
            {
                public Supplier Supplier { get; set; }

                public List<ProductEntry> ProductEntries { get; set; } = new List<ProductEntry>();

                public class ProductEntry
                {
                    public Product Product { get; set; }

                    public List<Order> Orders { get; set; } = new List<Order>();
                }
            }
        }

        public class Row
        {
            public string SupplierId { get; set; }
            public string ProductId { get; set; }
            public List<Supplier> Suppliers { get; set; }
            public List<Product> Products { get; set; }
            public List<Order> Orders { get; set; }
        }

        public void Do()
        {
            var session = DocumentStoreHolder.Store.OpenSession();

            QueryTimings timings = new QueryTimings();

            List<string> suppliers = new List<string> { "suppliers/4-a", "suppliers/5-a" };

            var res = (from entry in session
                        .Query<Orders_ByProduct_BySupplier.Entry, Orders_ByProduct_BySupplier>()
                        .Customize(x => x.Timings(out timings))
                        .Where(x => x.Supplier.In(suppliers))

                       let supplier = RavenQuery.Load<Supplier>(entry.Supplier)
                       let product = RavenQuery.Load<Product>(entry.Product)
                       let order = RavenQuery.Load<Order>(entry.Order)

                       select new
                       {
                           Supplier = supplier,
                           Product = product,
                           Order = order
                       })

                .ToList();

            var x = res
                .GroupBy(x => new { SupplierId = x.Supplier.Id, ProductId = x.Product.Id },
                    (key, value) => new Row
                    {
                        SupplierId = key.SupplierId,
                        ProductId = key.ProductId,
                        Suppliers = value.ToList().Select(x => x.Supplier).ToList(),
                        Products = value.ToList().Select(x => x.Product).ToList(),
                        Orders = value.ToList().Select(x => x.Order).ToList()
                    })
                .ToList();

            Result result = new Result();
            foreach (Row row in x)
            {
                result.AddEntry(row.Suppliers.First(), row.Products.First(), row.Orders);
            }
            result.Print();

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
                    (key, value) => new
                    {
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
