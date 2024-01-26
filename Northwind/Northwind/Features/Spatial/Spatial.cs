using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes.Spatial;

namespace Northwind.Features.Spatial
{
    public static class Spatial
    {  
        public static void OrdersInParis()
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

        // Find all Employees in a 20km circle from 47.623473, -122.3060097 coordinates
        public static void EmployeesInSeattle()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            List<Employee> employeesInSeattle =
                session.Query<Employee>()
                    .Spatial(
                    factory => factory.Point(
                        x => x.Address.Location.Latitude,
                        x => x.Address.Location.Longitude),
                    criteria => criteria.RelatesToShape(
                        shapeWkt: "CIRCLE(-122.3060097 47.623473 d=20)",
                        relation: SpatialRelation.Within)
                    ).ToList();

            foreach (Employee employee in employeesInSeattle)
            {
                Console.WriteLine($"{employee.Id}");
            }
        }

        // Find all Employees in a 20km circle from 47.623473, -122.3060097 coordinates
        public static void EmployeesInSeattle2()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            List<Employee> employeesInSeattle =
                session.Query<Employee>()
                    .Spatial(
                factory => factory.Point(
                        x => x.Address.Location.Latitude,
                        x => x.Address.Location.Longitude),
                factory => factory.WithinRadius(20, 47.623473, - 122.3060097)
                    ).ToList();

            foreach (Employee employee in employeesInSeattle)
            {
                Console.WriteLine($"{employee.Id}");
            }
        }
    }
}
