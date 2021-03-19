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
            public void Add(Supplier supplier, Product product, List<Order> orders)
            {
                var s = Add(supplier);

                if (product.Id != null)
                {
                    var p = Add(s, product);

                    if (orders.Any())
                        p.Orders.AddRange(orders);
                }
            }

            public SupplierEntry Add(Supplier supplier)
            {
                var s = supplierEntries.FirstOrDefault(x => x.Supplier.Id == supplier.Id);
                if (s == null)
                {
                    s = new SupplierEntry();
                    s.Supplier = supplier;
                    supplierEntries.Add(s);
                }

                return s;
            }

            public ProductEntry Add(SupplierEntry s, Product product)
            {
                var p = s.ProductEntries.FirstOrDefault(x => x.Product.Id == product.Id);
                if (p == null)
                {
                    p = new ProductEntry();
                    p.Product = product;
                    s.ProductEntries.Add(p);
                }

                return p;
            }

            public void Print()
            {
                foreach (var supplierEntry in supplierEntries)
                {
                    Console.WriteLine($"Supplier: {supplierEntry.Supplier.Id} {supplierEntry.Supplier.Name}");

                    foreach (var productEntry in supplierEntry.ProductEntries)
                    {
                        Console.WriteLine($"\t Product: [{productEntry.Product.Id}] {productEntry.Product.Name}");

                        foreach (var order in productEntry.Orders)
                        {
                            Console.WriteLine($"\t\t Order: {order.Id}");
                        }
                    }
                }

                Console.WriteLine();
                Console.WriteLine();
            }

            public void Print2()
            {
                foreach (var supplierEntry in supplierEntries)
                {
                    Console.WriteLine($"Supplier: {supplierEntry.Supplier.Id} {supplierEntry.Supplier.Name}");

                    foreach (var productEntry in supplierEntry.ProductEntries)
                    {
                        Console.WriteLine($"\t Product: [{productEntry.Product.Id}] {productEntry.Product.Name}");
                        Console.WriteLine($"\t\t Orders: {productEntry.Orders.Count}");
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
            }

            public class ProductEntry
            {
                public Product Product { get; set; }

                public List<Order> Orders { get; set; } = new List<Order>();
            }
        }

        public class Row
        {
            public string SupplierId { get; set; }
            public string ProductId { get; set; }
            public Supplier Supplier { get; set; }
            public Product Product { get; set; }
            public List<Order> Orders { get; set; }
        }

        public void Do()
        {
            Result result = new Result();

            var session = DocumentStoreHolder.Store.OpenSession();

            QueryTimings timings = new QueryTimings();

            var res = (from entry in session
                .Query<Products_BySupplier_ByOrder.Entry, Products_BySupplier_ByOrder>()
                .Customize(x => x.Timings(out timings))
                       let supplier = RavenQuery.Load<Supplier>(entry.Supplier)
                       let product = RavenQuery.Load<Product>(entry.Product)
                       let orders = RavenQuery.Load<Order>(entry.OrderIds).ToList()

                       select new
                       {
                           Supplier = supplier,
                           Product = product,
                           Order = orders,
                           OrderIds = entry.OrderIds
                       })
                .ToList();

            var x = res
                .GroupBy(x => new { SupplierId = x.Supplier.Id, ProductId = x.Product.Id },
                    (key, value) => new Row
                    {
                        SupplierId = key.SupplierId,
                        ProductId = key.ProductId,
                        Supplier = value.First().Supplier,
                        Product = value.First().Product,
                        Orders = value.ToList().SelectMany(x => x.Order).ToList()
                    })
                .ToList();

            foreach (Row row in x)
            {
                result.Add(row.Supplier, row.Product, row.Orders);
            }

            result.Print2();

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
            Console.WriteLine($"Total execution time: {timings.DurationInMs}ms");
        }
    }

    public class Products_BySupplier_ByOrder : AbstractMultiMapIndexCreationTask<Products_BySupplier_ByOrder.Entry>
    {
        public class Entry
        {
            public string Product { get; set; }
            public string Supplier { get; set; }
            public int Count { get; set; }
            public string[] OrderIds { get; set; }
        }

        public Products_BySupplier_ByOrder()
        {
            AddMap<Order>(orders =>
                from order in orders
                let items = order.Lines
                    .Select(ol => LoadDocument<Product>(ol.Product))
                    .Select(p => new { P = p, S = LoadDocument<Supplier>(p.Supplier) })
                from kvp in items
                select new Entry
                {
                    Product = kvp.P.Id,
                    Supplier = kvp.S.Id,
                    Count = 1,
                    OrderIds = new[] { order.Id }
                });

            AddMap<Product>(products =>
                from p in products
                let s = LoadDocument<Supplier>(p.Supplier)
                select new Entry
                {
                    Product = p.Id,
                    Supplier = s.Id,
                    Count = 0,
                    OrderIds = new string[0]
                });

            AddMap<Supplier>(suppliers =>
                from s in suppliers
                select new Entry
                {
                    Product = "",
                    Supplier = s.Id,
                    Count = 0,
                    OrderIds = new string[0]
                });

            Reduce = results => from result in results
                                group result by new { result.Product, result.Supplier }
                into g
                                select new
                                {
                                    Product = g.Key.Product,
                                    Supplier = g.Key.Supplier,
                                    Count = g.Sum(x => x.Count),
                                    OrderIds = g.SelectMany(x => x.OrderIds)
                                };

            Stores.Add(x => x.Product, FieldStorage.Yes);
            Stores.Add(x => x.Supplier, FieldStorage.Yes);
            Stores.Add(x => x.OrderIds, FieldStorage.Yes);
        }
    }
}
