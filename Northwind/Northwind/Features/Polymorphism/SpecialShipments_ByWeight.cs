using System.Linq;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Linq.Indexing;

namespace Northwind.Features.Polymorphism
{
    public class SpecialShipments_ByWeight : AbstractIndexCreationTask<SpecialShipment, SpecialShipments_ByWeight.Entry>
    {
        public class Entry
        {
            public double TotalWeight { get; set; }
        }

        public SpecialShipments_ByWeight()
        {
            Map = ss => ss.Select(shipment =>
                new Entry
                {
                    TotalWeight = ((Crate)shipment.Item).Weight != null ?
                        ((Crate)shipment.Item).Weight : 555
                }
            );
        }
    }
}
