using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace Northwind.Features.Indexes
{
    public class Orders_ByCompany : AbstractIndexCreationTask<Order, Orders_ByCompany.Entry>
    {
        public class Entry
        {
            public string CompanyName { get; set; }

            public string Employee { get; set; }

            public string Boss { get; set; }

            public decimal Freight { get; set; }
        }

        public Orders_ByCompany()
        {
            Map = orders => from order in orders
                            from orderLine in order.Lines
                            let employee = LoadDocument<Employee>(order.Employee)
                            let company = LoadDocument<Company>(order.Company)
                            select new Entry
                            {
                                CompanyName = company.Name,
                                Freight = order.Freight,
                                Employee = order.Employee,
                                Boss = employee.ReportsTo
                            };
        }
    }
}
