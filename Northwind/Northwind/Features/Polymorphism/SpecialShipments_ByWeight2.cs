using System;
using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Polymorphism
{
    /// <summary>
    /// In this version, we are using $type implicit discriminator generated
    /// by Json.NET - hence c# client specific
    /// </summary>
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
