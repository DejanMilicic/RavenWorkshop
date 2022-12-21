using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.InnerJoin
{
    public class Vehicles : AbstractMultiMapIndexCreationTask<Vehicles.Entry>
    {
        public class Entry
        {
            public string RegNo { get; set; }

            public string VehicleId { get; set; }

            public string OwnerId { get; set; }

            public string InsuranceId { get; set; }
        }

        public Vehicles()
        {
            AddMap<VehicleDetails>(vehicles => from vd in vehicles
                select new Entry
                {
                    RegNo = vd.RegNo,
                    VehicleId = vd.Id,
                    OwnerId = "",
                    InsuranceId = ""
                });

            AddMap<VehicleOwnerDetails>(owners => from owner in owners
                select new Entry
                {
                    RegNo = owner.RegNo,
                    VehicleId = "",
                    OwnerId = owner.Id,
                    InsuranceId = ""
                });

            AddMap<VehicleInsuranceDetails>(insurances => from insurance in insurances
                select new Entry
                {
                    RegNo = insurance.RegNo,
                    VehicleId = "",
                    OwnerId = "",
                    InsuranceId = insurance.Id
                });

            Reduce = results => from result in results
                group result by new
                {
                    result.RegNo
                }
                into g
                select new Entry
                {
                    RegNo = g.Key.RegNo,
                    VehicleId = g.FirstOrDefault(x => x.VehicleId != "").VehicleId,
                    OwnerId = g.FirstOrDefault(x => x.OwnerId != "").OwnerId,
                    InsuranceId = g.FirstOrDefault(x => x.InsuranceId != "").InsuranceId
                };

            OutputReduceToCollection = "FullVehicleDetails";
        }
    }
}
