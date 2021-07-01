using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Polymorphism
{
    public class Shipments_ByWeight : AbstractIndexCreationTask<Shipment, Shipments_ByWeight.Entry>
    {
        public class Entry
        {
            public double TotalWeight { get; set; }
        }

        public Shipments_ByWeight()
        {
            Map = shipments => shipments.Select(shipment =>
                new Entry
                {
                    TotalWeight = shipment.Items.OfType<Crate>().Sum(x => x.Weight)
                }
            );
        }
    }
}
