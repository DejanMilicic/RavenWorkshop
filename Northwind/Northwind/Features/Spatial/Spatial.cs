using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;

namespace Northwind.Features.Spatial
{
    public class Spatial
    {
        public void OrdersInParis()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            List<Order> orders = session.Advanced.RawQuery<Order>(@"
                from index 'Orders/ByShipment/Location'
                where spatial.within(ShipmentLocation, spatial.circle(10, 48.8566, 2.3522))
            ").ToList();

            foreach (Order order in orders)
            {
                Console.WriteLine($"{order.Id}");
            }
        }
    }
}
