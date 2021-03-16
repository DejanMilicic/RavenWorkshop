using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents.Queries;

namespace Northwind.Features.Projections
{
    public class Projections
    {
        public void Projection1()
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

        public void Projection2()
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
    }
}
