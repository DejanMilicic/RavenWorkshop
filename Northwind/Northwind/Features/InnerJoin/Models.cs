using System;

namespace Northwind.Features.InnerJoin
{
    public class VehicleDetails
    {
        public string Id { get; set; }

        public string RegNo { get; set; }

        public string ChassisNo { get; set; }

        public string EngineNo { get; set; }

        public DateTime Registration { get; set; }
    }
    
    public class VehicleOwnerDetails
    {
        public string Id { get; set; }

        public string RegNo { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public DateTime Dob { get; set; }
    }

    public class VehicleInsuranceDetails
    {
        public string Id { get; set; }

        public string RegNo { get; set; }
        
        public string CompanyName { get; set; }

        public string CompanyAddress { get; set; }
        
        public DateTime InsuranceExpiration { get; set; }
    }

    public class FullVehicleInfo
    {
        public string RegNo { get; set; }

        public string ChassisNo { get; set; }

        public string EngineNo { get; set; }

        public DateTime Registration { get; set; }

        public string OwnerName { get; set; }

        public string OwnerAddress { get; set; }

        public DateTime OwnerDob { get; set; }

        public string InsuranceCompanyName { get; set; }

        public string InsuranceCompanyAddress { get; set; }

        public DateTime InsuranceExpiration { get; set; }
    }
}
