using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Polymorphism
{
    /// <summary>
    /// Here we are using Discriminator property introduced on a base class
    /// This is most elegant solution, and it is universal for all RavenDB clients
    /// </summary>
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
                    where s.Item.Discriminator == nameof(Crate)
                select new Entry
                {
                    TotalWeight = ((Crate)s.Item).Weight
                }
            );

            // car
            AddMap<SpecialShipment>(ss =>
                from s in ss
                    where s.Item.Discriminator == nameof(Car)
                select new Entry
                {
                    TotalWeight = 555
                }
            );
        }
    }
}
