using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Northwind.Features.Indexes;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session.Loaders;

namespace Northwind.Features
{
    public partial class Examples
    {
        public void IncludeViaPath()
        {
            var store = DocumentStoreHolder.GetStore().Initialize();

            using var session = store.OpenSession();

            var orders = session.Query<Order>()
                .Include("Employee")
                .Include("Company")
                .Where(order => order.Company == "Companies/2-A")
                .OrderByDescending(order => order.OrderedAt)
                .Take(10)
                .ToList();

            foreach (Order order in orders)
            {
                var employee = session.Load<Employee>(order.Employee);
                var company = session.Load<Company>(order.Company);
                Console.WriteLine($"Order: {order.Id} \t {order.OrderedAt} \t via {employee.FirstName} \t for {company.Name}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void IncludeViaPathList(string[] includes)
        {
            var store = DocumentStoreHolder.GetStore().Initialize();

            using var session = store.OpenSession();

            var query = session.Query<Order>();

            foreach (string incl in includes)
                query = query.Include(incl);

            var orders = query
                .Where(order => order.Company == "Companies/2-A")
                .OrderByDescending(order => order.OrderedAt)
                .Take(10)
                .ToList();

            foreach (Order order in orders)
            {
                var employee = session.Load<Employee>(order.Employee);
                var company = session.Load<Company>(order.Company);
                Console.WriteLine($"Order: {order.Id} \t {order.OrderedAt} \t via {employee.FirstName} \t for {company.Name}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void LoadAndIncludeViaPathList(string[] includes)
        {
            var store = DocumentStoreHolder.GetStore().Initialize();

            using var session = store.OpenSession();

            Order order;

            if (includes.Any())
            {
                ILoaderWithInclude<object> loader = session.Include(includes.First());

                foreach (string include in includes.Skip(1))
                {
                    loader = loader.Include(include);
                }

                order = loader.Load<Order>("Orders/61-A");
            }
            else
            {
                order = session.Load<Order>("Orders/61-A");
            }
            
            var employee = session.Load<Employee>(order.Employee);
            var company = session.Load<Company>(order.Company);

            Console.WriteLine($"Order: {order.Id} \t {order.OrderedAt} \t via {employee.FirstName} \t for {company.Name}");

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void SecondLevelInclude2()
        {
            var store = DocumentStoreHolder.GetStore().Initialize();

            using var session = store.OpenSession();

            string query = @"
declare function output(o) {
    include(o.Employee);
    var emp = load(o.Employee);
    include(emp.ReportsTo);

	return o
}

from Orders as o
where o.Company = 'Companies/2-A'
select output(o)
";

            var orders = session.Advanced.RawQuery<Order>(query).ToList();

            foreach (Order order in orders)
            {
                var employee = session.Load<Employee>(order.Employee);
                var reportsTo = session.Load<Employee>(employee.ReportsTo);
                Console.WriteLine($"Order: {order.Id} \t {order.OrderedAt} \t via {employee.FirstName} \t reports to {reportsTo.FirstName}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void SecondLevelProjection()
        {
            var store = DocumentStoreHolder.GetStore().Initialize();

            using var session = store.OpenSession();

            var results = (from o in session.Query<Order>()
                let e = RavenQuery.Load<Employee>(o.Employee)
                let r = RavenQuery.Load<Employee>(e.ReportsTo)
                select new
                {
                    Order = o,
                    Employee = e,
                    ReportsTo = r
                }).ToList();

            foreach (var entry in results)
            {
                Console.WriteLine($"Order: {entry.Order.Id} \t {entry.Order.OrderedAt} \t via {entry.Employee.FirstName} \t reports to {entry.ReportsTo?.FirstName}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void ProjectionViaJS()
        {
            var store = DocumentStoreHolder.GetStore().Initialize();

            using var session = store.OpenSession();

            string query = @"
declare function output(o) {
    var lines = o.Lines;
    var products = [];
    var suppliers = [];
    lines.forEach(line => {
        var product = load(line.Product);
        if (product) {
            products.push(product);
            var supplier = load(product.Supplier);
            if (supplier) {
                suppliers.push(supplier);
            }
        }
    });
	return {
	    Order: o,
	    Products: products,
	    Suppliers: suppliers
	}
}
from Orders as o
select output(o)
";
            var r = session.Advanced.RawQuery<Result>(query).ToList();

            var orders = r.Select(x => x.Order).ToDictionary(x => x.Id, x => x);
            var products = r.SelectMany(x => x.Products)
                .GroupBy(p => p.Id, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            var suppliers = r.SelectMany(x => x.Suppliers)
                .GroupBy(p => p.Id, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            foreach (Supplier supplier in suppliers.Values)
            { 
                Console.WriteLine($"Supplier: \t {supplier.Name}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void SecondLevelInclude()
        {
            var store = DocumentStoreHolder.GetStore().Initialize();

            using var session = store.OpenSession();

            string query = @"
declare function output(o) {
    o.Lines.forEach(line => {
        include(line.Product)
        var product = load(line.Product);
        include(product.Supplier);
    });
	return o
}

from Orders as o
select output(o)
";
            var orders = session.Advanced.RawQuery<Order>(query).ToList();

            foreach (Order order in orders)
            {
                var products = session.Load<Product>(order.Lines.Select(x => x.Product));

                foreach (Product product in products.Values)
                {
                    var supplier = session.Load<Supplier>(product.Supplier);
                    Console.WriteLine($"Order: {order.Id} \t {order.OrderedAt} \t via {supplier.Name}");
                }
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void OrdersProjection(string companyName)
        {
            var store = DocumentStoreHolder.GetStore().Initialize();

            using var session = store.OpenSession();

            var entries = session.Query<Orders_ByCompany.Entry, Orders_ByCompany>()
                .Where(x => x.CompanyName == companyName)
                .ProjectInto<Orders_ByCompany.Entry>()
                .Select(x => 
                    new { 
                    Order = RavenQuery.Load<Order>(x.OrderId),
                    Employee = RavenQuery.Load<Employee>(x.Employee),
                    Boss = RavenQuery.Load<Employee>(x.Boss)
                })
                .ToList();

            foreach (var entry in entries)
            {
                Console.WriteLine($"{entry.Order.Id} by {entry.Employee.FirstName} who reports to {entry.Boss?.FirstName ?? "None"}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void OrdersProjectionJustFields(string companyName)
        {
            var store = DocumentStoreHolder.GetStore().Initialize();

            using var session = store.OpenSession();

            var entries = (from entry in session.Query<Orders_ByCompany.Entry, Orders_ByCompany>()
                          where entry.CompanyName == companyName
                          let order = RavenQuery.Load<Order>(entry.OrderId)
                          let employee = RavenQuery.Load<Employee>(entry.Employee)
                          let boss = RavenQuery.Load<Employee>(entry.Boss)
                          select new 
                            {
                                OrderFreight = order.Freight,
                                Employee = new { 
                                    First = employee.FirstName,
                                    Last = employee.LastName
                                },
                                Boss = boss != null ? (boss.FirstName + " " + boss.LastName) : "Himself :)"
                            })
                    .ToList();

            foreach (var entry in entries)
            {
                Console.WriteLine($"{entry.OrderFreight} by {entry.Employee.First + " " + entry.Employee.Last} who reports to {entry.Boss}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void OrdersInclude(string companyName)
        {
            var store = DocumentStoreHolder.GetStore().Initialize();

            using var session = store.OpenSession();

            var entries = session.Query<Orders_ByCompany.Entry, Orders_ByCompany>()
                .Where(x => x.CompanyName == companyName)
                .ProjectInto<Orders_ByCompany.Entry>()
                .Include(x => x.OrderId)
                .Include(x => x.Employee)
                .Include(x => x.Boss)
                .ToList();

            foreach (var entry in entries)
            {
                Employee employee = session.Load<Employee>(entry.Employee);
                Employee boss = session.Load<Employee>(employee.ReportsTo);
                Order order = session.Load<Order>(entry.OrderId);

                Console.WriteLine($"{order.Id} by {employee.FirstName} who reports to {boss?.FirstName ?? "None"}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public class Result
        {
            public Order Order { get; set; }

            public List<Product> Products { get; set; }

            public List<Supplier> Suppliers { get; set; }
        }
    }
}
