using System;
using Raven.Client.Documents;

namespace Northwind.Features.InnerJoin
{
    public static class Seed
    {
        public static void Data(IDocumentStore store)
        {
            using var session = store.OpenSession();

            string regNo = "ABC-11";

            VehicleDetails vd1 = new VehicleDetails
            {
                RegNo = regNo,
                ChassisNo = "ch-1",
                EngineNo = "en-1",
                Registration = new DateTime(2011, 1, 1)
            };
            session.Store(vd1);

            VehicleOwnerDetails vo1 = new VehicleOwnerDetails
            {
                RegNo = regNo,
                Address = "1 First Street",
                Name = "Person First",
                Dob = new DateTime(2001, 1, 1)
            };
            session.Store(vo1);

            VehicleInsuranceDetails in1 = new VehicleInsuranceDetails
            {
                RegNo = regNo,
                CompanyName = "Company 1",
                CompanyAddress = "1 Corporate Way",
                InsuranceExpiration = new DateTime(2011, 11, 11)
            };
            session.Store(in1);

            session.SaveChanges();
        }
    }
}
