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
    }
}
