using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;

namespace Northwind.Features.Create
{
    public static class Create
    {
        public static void ObjectWithList()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Order order = new Order();
            order.Company = "companies/91-A";
            order.Employee = "employees/8-A";
            
            order.Lines = new List<OrderLine>();
            order.Lines.Add(new OrderLine
            {
                Product = "products/77-A",
                ProductName = "Original Frankfurter grüne Soße",
                PricePerUnit = 10,
                Quantity = 1
            });

            session.Store(order);
            session.SaveChanges();
        }
    }
}
