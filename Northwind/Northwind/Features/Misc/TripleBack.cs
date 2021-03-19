using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Queries.Timings;
using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents;

namespace Northwind.Features.Misc
{
    public class TripleBack
    {
        public class Result
        {
            public void Add(IEnumerable<Supplier> suppliers)
            {
                foreach (Supplier supplier in suppliers)
                    Add(supplier);
            }

            public void Add(Supplier supplier, Product product, List<Order> orders)
            {
                var s = Add(supplier);
                var p = Add(s, product);
                p.Orders.AddRange(orders);
            }

            public void Add(Supplier supplier, IEnumerable<Product> products)
            {
                var s = Add(supplier);

                foreach (Product product in products)
                {
                    Add(s, product);
                }
            }

            public void Add(Supplier supplier, Product product)
            {
                var s = Add(supplier);
                Add(s, product);
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
            public List<Supplier> Suppliers { get; set; }
            public List<Product> Products { get; set; }
            public List<Order> Orders { get; set; }
        }

        public void Do()
        {
            Result result = new Result();

            var session = DocumentStoreHolder.Store.OpenSession();

            QueryTimings timings = new QueryTimings();






            result.Print();

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
            Console.WriteLine($"Total execution time: {timings.DurationInMs}ms");
        }

        public void Do3()
        {
            Result result = new Result();

            var session = DocumentStoreHolder.Store.OpenSession();

            QueryTimings timings = new QueryTimings();

            //List<string> suppliers = new List<string> { "", "suppliers/5-a" };

            List<Supplier> suppliers = new List<Supplier>();
            suppliers.AddRange(session.Query<Supplier>().Where(x => x.Id.In(new string[] {"suppliers/4-a", "Suppliers/0000000000000008434-A"})));
            //suppliers.AddRange((IEnumerable<Supplier>)session.Load<Supplier>());
            //var suppliers = session.Query<Supplier>().Skip(2).Take(5).ToList();
            result.Add(suppliers);

            var allproducts = session.Query<Product>()
                .Where(x => x.Supplier.In(suppliers.Select(x => x.Id)))
                .ToList();

            foreach (Product product in allproducts)
            {
                Supplier supplier = session.Load<Supplier>(product.Supplier);
                result.Add(supplier, product);
            }
            
            var res = (from entry in session
                        .Query<Orders_ByProduct_BySupplier.Entry, Orders_ByProduct_BySupplier>()
                        .Customize(x => x.Timings(out timings))
                        .Where(x => x.Supplier.In(suppliers.Select(x => x.Id)))

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

            foreach (Row row in x)
            {
                result.Add(row.Suppliers.First(), row.Products.First(), row.Orders);
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

    public class Products_BySupplier_ByOrder : AbstractMultiMapIndexCreationTask
    {
        public class Entry
        {
            public string Order { get; set; }

            public string Product { get; set; }

            public string Supplier { get; set; }
        }

        public Products_BySupplier_ByOrder()
        {
            //AddMap<Order>(
                
            //    );
        }
    }
}

/*
  
from order in docs.Orders
let items = order.Lines.Select(ol => LoadDocument(ol.Product, "Products")).Select(p => new { P = p, S = LoadDocument(p.Supplier, "Suppliers")})
from kvp in items
select new
{
    Product = kvp.P.Id,
    Supplier = kvp.S.Id,
    Count = 1,
    OrderIds = new [] { order.Id }
}  

from p in docs.Products
let s = LoadDocument(p.Supplier, "Suppliers")
select new {
    Product = p.Id,
    Supplier = s.Id,
    Count = 0,
    OrderIds = new string[0]
}

from result in results
group result by new { result.Product, result.Supplier } into g
select new {
    Product = g.Key.Product,
    Supplier = g.Key.Supplier,
    Count = g.Sum(x => x.Count),
    OrderIds = g.SelectMany(x => x.OrderIds)
}

 */