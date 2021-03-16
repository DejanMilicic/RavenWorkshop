using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents;

namespace Northwind.Features.Includes
{
    public class Includes
    {
        public void LoadWithInclude()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var order = session
                .Include<Order>(x => x.Company)
                .Load("orders/1-A");

            var company = session.Load<Company>(order.Company);

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void ManyCalls()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var orders = session.Query<Order>()
                .Where(order => order.Company == "Companies/2-A")
                .ToList();

            foreach (Order order in orders)
            {
                var employee = session.Load<Employee>(order.Employee);
                var company = session.Load<Company>(order.Company);
                Console.WriteLine($"Order: {order.Id} \t {order.OrderedAt} \t via {employee.FirstName} \t for {company.Name}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void ManyCallsOptimizedViaInclude()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var orders = session.Query<Order>()
                .Include(x => x.Employee)
                .Include(x => x.Company)
                .Where(order => order.Company == "Companies/2-A")
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
