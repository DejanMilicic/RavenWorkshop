using System;
using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents;

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
    }
}
