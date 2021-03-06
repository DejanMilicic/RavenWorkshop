using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
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

        public void ProjectionViaJS()
        {
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

        public class Result
        {
            public Order Order { get; set; }

            public List<Product> Products { get; set; }

            public List<Supplier> Suppliers { get; set; }
        }
    }
}
