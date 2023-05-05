using System.Collections.Generic;
using Raven.Client.Documents.Indexes;
using System.Linq;
using FluentAssertions;
using Northwind.Models.Entity;

namespace Northwind.Features.Indexes.Recursive;

public class Orders_ByTopManager : AbstractIndexCreationTask<Order, Orders_ByTopManager.Entry>
{
    public class Entry
    {
        public List<Employee> Managers { get; set; }

        public Employee TopManager { get; set; }

        public Employee BottomManager { get; set; }
    }

    public Orders_ByTopManager()
    {
        Map = orders => from order in orders
            select new Entry
            {
                TopManager = 
                    Recurse(
                        LoadDocument<Employee>(order.Employee), 
                        x => LoadDocument<Employee>(x.ReportsTo))
                        .Last(),

                Managers = 
                    Recurse(
                        LoadDocument<Employee>(order.Employee), 
                        x => LoadDocument<Employee>(x.ReportsTo))
                        .ToList()
                        
            };
    }
}

