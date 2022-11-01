using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Features.Indexes;
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
        // todo : test these two
        public void Example1()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            // todo pull common subexpression
            var orders = session
                .Query<Orders_ByEmployeeLastName.Entry, Orders_ByEmployeeLastName>()
                .Where(x => x.LastName == "Davolio")
                .OfType<Order>() // will filter out those of specified type
                .ToList();

            var orders2 = session
                .Query<Orders_ByEmployeeLastName.Entry, Orders_ByEmployeeLastName>()
                .Where(x => x.LastName == "Davolio")
                .As<Order>() // will throw on objects that cannot be casted
                .ToList();

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }
    }

    public static class ProjectionsDemo
    {
        public static void Do()
        {
            // todo : join with example above

            using var session = DocumentStoreHolder.Store.OpenSession();

            List<Order> orders = session.Query<Orders_ByCompany.Entry, Orders_ByCompany>()
                .Where(x => x.CompanyName == "Rattlesnake Canyon Grocery")
                .ProjectInto<Order>()
                .ToList();

            List<Order> orders2 = session.Query<Orders_ByCompany.Entry, Orders_ByCompany>()
                .Where(x => x.CompanyName == "Rattlesnake Canyon Grocery")
                .OfType<Order>()
                .ToList();
        }
    }
}
