using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Polymorphism
{
    /// <summary>
    /// Attempt at reading specific property from Crate is used
    /// to determine type
    /// </summary>
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
