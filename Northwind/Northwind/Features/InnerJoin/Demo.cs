using System;
using System.Linq;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents;
using Northwind.Models.Entity;
using Raven.Client.Documents.Queries;

namespace Northwind.Features.InnerJoin
{
    public static class InnerJoin
    {
        public static void Demo()
        {
            var store = new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "Transport"
            }.Initialize();

            IndexCreation.CreateIndexes(new AbstractIndexCreationTask[] { new Vehicles() }, store);

            // uncomment to seed
            //Seed.Data(store);

            using var session = store.OpenSession();

            FullVehicleInfo vi = (
                    from v in session.Query<Vehicle>()
                    where v.RegNo == "ABC-11"
                    let vd = RavenQuery.Load<VehicleDetails>(v.VehicleDetailsId)
                    let od = RavenQuery.Load<VehicleOwnerDetails>(v.OwnerId)
                    let id = RavenQuery.Load<VehicleInsuranceDetails>(v.InsuranceId)
                    select new FullVehicleInfo
                    {
                        RegNo = vd.RegNo,
                        ChassisNo = vd.ChassisNo,
                        EngineNo = vd.EngineNo,
                        Registration = vd.Registration,
                        OwnerName = od.Name,
                        OwnerAddress = od.Address,
                        OwnerDob = od.Dob,
                        InsuranceCompanyName = id.CompanyName,
                        InsuranceCompanyAddress = id.CompanyAddress,
                        InsuranceExpiration = id.InsuranceExpiration
                    })
                    .Single();

            vi.Print();
        }
    }
}
