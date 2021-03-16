using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents;

namespace Northwind.Features.Lazy
{
    public class Lazy
    {
        public void ManyCalls()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee employee = session.Load<Employee>("employees/9-A");
            List<Order> orders = session.Query<Order>()
                .Where(x => x.Employee == "employees/9-A")
                .ToList();

            foreach (Order order in orders)
            {
                Console.WriteLine($"Order: {order.Id}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void ManyCallsOptimizedViaLazily()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Lazy<Employee> employee = session.Advanced.Lazily.Load<Employee>("employees/9-A");
            Lazy<IEnumerable<Order>> orders = session.Query<Order>()
                .Where(x => x.Employee == "employees/9-A")
                .Lazily();

            foreach (Order order in orders.Value)
            {
                Console.WriteLine($"Order: {order.Id}");
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void ExecutePending()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var order = session.Advanced.Lazily.Load<Order>("Orders/61-A");
            var employee = session.Advanced.Lazily.Load<Employee>(order.Value.Employee);
            var company = session.Advanced.Lazily.Load<Company>(order.Value.Company);

            session.Advanced.Eagerly.ExecuteAllPendingLazyOperations();

            Console.WriteLine($"Order: {order.Value.Id} \t {order.Value.OrderedAt} \t via {employee.Value.FirstName} \t for {company.Value.Name}");

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }
    }
}
