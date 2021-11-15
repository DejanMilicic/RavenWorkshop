using System.Linq;
using System.Net.Sockets;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Linq.Indexing;

namespace Northwind.Features.Polymorphism
{
    public class SpecialShipments_ByWeight3 : 
        AbstractMultiMapIndexCreationTask<SpecialShipment>
    {
        public class Entry
        {
            public double TotalWeight { get; set; }
        }

        public SpecialShipments_ByWeight3()
        {
            // crate
            AddMap<SpecialShipment>(ss => 
                from s in ss 
                    where AsJson(s.Item)["Discriminator"].ToString() == nameof(Crate)
                select new Entry
                {
                    TotalWeight = ((Crate)s.Item).Weight
                }
            );

            // car
            AddMap<SpecialShipment>(ss =>
                from s in ss
                    where AsJson(s.Item)["Discriminator"].ToString() == nameof(Car)
                select new Entry
                {
                    TotalWeight = 555
                }
            );
        }
    }
}
