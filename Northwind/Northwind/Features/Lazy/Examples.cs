using System;
using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents;

namespace Northwind.Features
{
    public partial class Examples
    {
        public void LazyExample(string[] includes)
        {
            using var session = store.OpenSession();

            var order = session.Advanced.Lazily.Load<Order>("Orders/61-A");
            var employee = session.Advanced.Lazily.Load<Employee>(order.Value.Employee);
            //var company = session.Advanced.Lazily.Load<Company>(order.Value.Company);

            session.Advanced.Eagerly.ExecuteAllPendingLazyOperations();

            Console.WriteLine($"Order: {order.Value.Id} \t {order.Value.OrderedAt} \t via {employee.Value.FirstName} \t for ");

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }
    }
}
