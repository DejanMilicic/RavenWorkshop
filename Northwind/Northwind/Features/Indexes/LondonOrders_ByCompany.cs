using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace Northwind.Features.Indexes
{
    public class LondonOrders_ByCompany : AbstractIndexCreationTask<Order, LondonOrders_ByCompany.Entry>
    {
        public class Entry
        {
            public string CompanyName { get; set; }

            public string Employee { get; set; }

            public string Boss { get; set; }

            public decimal Freight { get; set; }
        }

        public LondonOrders_ByCompany()
        {
            Map = orders => from order in orders
                            where order.ShipTo.City == "London"
                            let employee = LoadDocument<Employee>(order.Employee)
                            let company = LoadDocument<Company>(order.Company)
                            select new Entry
                            {
                                CompanyName = company.Name,
                                Freight = order.Freight,
                                Employee = employee.Id,
                                Boss = employee.ReportsTo
                            };
        }
    }
}
