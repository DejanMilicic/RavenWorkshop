using System;
using System.Collections.Generic;
using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Queries;

namespace Northwind.Features.Projections
{
    public static class Projections
    {
        public static void Projection1()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var orders = session.Query<Order>()
                .Where(x => x.Employee == "employees/2-A")
                .OrderByDescending(x => x.OrderedAt)
                .Take(5)
                .Select(x => new
                {
                    x.Id,
                    EmpFirstName = RavenQuery.Load<Employee>(x.Employee).FirstName
                })
                .ToList();

            foreach (var order in orders)
            {
                Console.WriteLine($"{order.Id} by {order.EmpFirstName}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public static void Projection2()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var projectionResults = from order in session.Query<Order>()
                where order.ShipTo.City == "London"
                let company = RavenQuery.Load<Company>(order.Company)
                let employee = RavenQuery.Load<Employee>(order.Employee)
                select new
                {
                    company.Name,
                    order.ShipTo.City,
                    employee.FirstName
                };

            var results = projectionResults.ToList();

            foreach (var result in results)
            {
                Console.WriteLine($"Company {result.Name} from {result.City} via {result.FirstName}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public static void Projection3()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var projectionResults = from order in session.Query<Order>()
                where order.ShipTo.City == "London"
                let company = RavenQuery.Load<Company>(order.Company)
                let employee = RavenQuery.Load<Employee>(order.Employee)
                select new
                {
                    company.Name,
                    order.ShipTo.City,
                    EmployeeName = employee.FirstName + " " + employee.LastName
                };

            var results = projectionResults.ToList();

            foreach (var result in results)
            {
                Console.WriteLine($"Company {result.Name} from {result.City} via {result.EmployeeName}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public static void Projection4()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var projectionResults = from order in session.Query<Order>()
                where order.ShipTo.City == "London"
                let company = RavenQuery.Load<Company>(order.Company)
                let employee = RavenQuery.Load<Employee>(order.Employee)
                let fullName =
                    new Func<Employee, string>(e => e.FirstName + " " + e.LastName)
                select new
                {
                    company.Name,
                    order.ShipTo.City,
                    Employee = fullName(employee)
                };


            var results = projectionResults.ToList();

            foreach (var result in results)
            {
                Console.WriteLine($"Company {result.Name} from {result.City} via {result.Employee}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public class OrderViewModel
        {
            public string OrderId { get; set; }
            public string Company { get; set; }
            public string Employee { get; set; }
            public string ShippingCity { get; set; }
        }

        // todo : example of projecting metadata properties

        public static void ProjectionWithPermissions()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            bool isAdmin = false; // try changing to true and observing result

            List<OrderViewModel> londonOrders = (
                    from order in session.Query<Order>()
                    where order.ShipTo.City == "London"
                    let company = RavenQuery.Load<Company>(order.Company)
                    let employee = RavenQuery.Load<Employee>(order.Employee)
                    select new OrderViewModel
                    {
                        OrderId = order.Id,
                        Company = isAdmin ? company.Name : "[REDACTED]",
                        Employee = employee.FirstName + " " + employee.LastName,
                        ShippingCity = order.ShipTo.City
                    }
                    ).ToList();

            foreach (OrderViewModel order in londonOrders)
            {
                Console.WriteLine($"Company {order.Company} from {order.ShippingCity} via {order.Employee}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public static void ProjectionWithDynamicFields()
        {
            using var store = (new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "demo"
            }).Initialize();

            using var session = store.OpenSession();

            var result = 
                session.Advanced.DocumentQuery<Employee>()
                .SelectFields<object>(
                    new QueryData(
                        new[] { nameof(Employee.FirstName), nameof(Employee.LastName) }, 
                        new[] { "first", "last" }))
                .ToList();
        }
    }
}
