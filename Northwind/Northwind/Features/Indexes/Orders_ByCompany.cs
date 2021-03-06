﻿using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace Northwind.Features.Indexes
{
    public class Orders_ByCompany : AbstractIndexCreationTask<Order, Orders_ByCompany.Entry>
    {
        public class Entry
        {
            public string OrderId { get; set; }

            public string CompanyName { get; set; }

            public string Employee { get; set; }

            public string Boss { get; set; }

            public decimal Freight { get; set; }
        }

        public Orders_ByCompany()
        {
            Map = orders => from order in orders
                            let employee = LoadDocument<Employee>(order.Employee)
                            let company = LoadDocument<Company>(order.Company)
                            select new Entry
                            {
                                OrderId = order.Id,
                                CompanyName = company.Name,
                                Freight = order.Freight,
                                Employee = employee.Id,
                                Boss = employee.ReportsTo
                            };

            Stores.Add(x => x.OrderId, FieldStorage.Yes);
            Stores.Add(x => x.CompanyName, FieldStorage.Yes);
            Stores.Add(x => x.Employee, FieldStorage.Yes);
            Stores.Add(x => x.Boss, FieldStorage.Yes);
        }
    }
}
