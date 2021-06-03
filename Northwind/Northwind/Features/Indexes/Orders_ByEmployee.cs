using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;
using System.Collections.Generic;
using System.Linq;

namespace Northwind.Features.Indexes
{
    public class Orders_ByEmployee : AbstractIndexCreationTask<Order, Orders_ByEmployee.Entry>
    {
        public class Entry
        {
            public string Employee { get; set; }

            public List<string> Orders { get; set; }
        }

        public Orders_ByEmployee()
        {
            Map = orders => from order in orders
                            let employee = LoadDocument<Employee>(order.Employee)
                            let company = LoadDocument<Company>(order.Company)
                            select new Entry
                            {
                                Employee = employee.Id,
                                Orders = new List<string> { order.Id }
                            };

            Reduce = results => from result in results
                                group result by new
                                {
                                    result.Employee
                                }
                    into g
                                select new Entry
                                {
                                    Employee = g.Key.Employee,
                                    Orders = g.SelectMany(x => x.Orders).ToList()
                                };

            Stores.Add(x => x.Orders, FieldStorage.Yes);
        }
    }
}
