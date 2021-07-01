using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.Polymorphism
{
    public class Polymorphism
    {
        public void Seed()
        {
            Shipment shipment = new Shipment();
            shipment.Items.Add(new Car());
            shipment.Items.Add(new Crate { Weight = 23 });
            shipment.Items.Add(new Crate { Weight = 13 });

            using var session = DocumentStoreHolder.Store.OpenSession();
            session.Store(shipment);
            session.SaveChanges();
        }
    }
}
