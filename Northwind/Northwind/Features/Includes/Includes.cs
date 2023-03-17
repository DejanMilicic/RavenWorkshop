using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
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

            List<Order> orders = session.Query<Order>()
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

        public void Select_N_plus_1()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();
            session.Advanced.MaxNumberOfRequestsPerSession = 80;

            List<Order> shippedOrders = session.Query<Order>()
                .Where(c => c.ShippedAt != null)
                .ToList();

            foreach (Order shippedOrder in shippedOrders)
            {
                List<string> productIds = shippedOrder.Lines.Select(x => x.Product).ToList();

                for (var i = 0; i < productIds.Count; i++)
                {
                    Product product = session.Load<Product>(productIds[i]);
                    // do something with product...
                }
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void Select_N_plus_1_solved()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            List<Order> shippedOrders = session.Query<Order>()
                .Include(c => c.Lines.Select(x => x.Product))
                .Where(c => c.ShippedAt != null)
                .ToList();

            foreach (Order shippedOrder in shippedOrders)
            {
                List<string> productIds = shippedOrder.Lines.Select(x => x.Product).ToList();

                for (var i = 0; i < productIds.Count; i++)
                {
                    Product product = session.Load<Product>(productIds[i]);
                    // do something with product...
                }
            }

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }
    }
}
