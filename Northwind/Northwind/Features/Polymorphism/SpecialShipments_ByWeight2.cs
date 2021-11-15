using System;
using System.Linq;
using System.Net.Sockets;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Linq.Indexing;

namespace Northwind.Features.Polymorphism
{
    public class SpecialShipments_ByWeight2 : 
        AbstractMultiMapIndexCreationTask<SpecialShipment>
    {
        public class Entry
        {
            public double TotalWeight { get; set; }
        }

        public SpecialShipments_ByWeight2()
        {
            // crate
            AddMap<SpecialShipment>(ss => 
                from s in ss 
                where AsJson(s.Item)["$type"].ToString()
                    .Split(',', StringSplitOptions.None).First()
                    .Split('.', StringSplitOptions.None).Last() == nameof(Crate)
                select new Entry
                {
                    TotalWeight = ((Crate)s.Item).Weight
                }
            );

            // car
            AddMap<SpecialShipment>(ss =>
                from s in ss
                where AsJson(s.Item)["$type"].ToString()
                    .Split(',', StringSplitOptions.None).First()
                    .Split('.', StringSplitOptions.None).Last() == nameof(Car)
                select new Entry
                {
                    TotalWeight = 555
                }
            );
        }
    }
}
