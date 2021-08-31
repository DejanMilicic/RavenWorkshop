using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Projections
{
    public class Orders_ByEmployeeLastName : AbstractIndexCreationTask<Order>
    {
        public class Entry
        {
            public string LastName { get; set; }
        }

        public Orders_ByEmployeeLastName()
        {
            Map = orders => from order in orders
                            let employee = LoadDocument<Employee>(order.Employee)
                            select new
                            {
                                LastName = employee.LastName
                            };

            StoreAllFields(FieldStorage.Yes);
        }
    }

    public class As
    {
        public void Example1()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var orders = session
                .Query<Orders_ByEmployeeLastName.Entry, Orders_ByEmployeeLastName>()
                .Where(x => x.LastName == "Davolio")
                .OfType<Order>()
                .ToList();

            var orders2 = session
                .Query<Orders_ByEmployeeLastName.Entry, Orders_ByEmployeeLastName>()
                .Where(x => x.LastName == "Davolio")
                .As<Order>()
                .ToList();

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }
    }
}
