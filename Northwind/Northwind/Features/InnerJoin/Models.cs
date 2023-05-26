using System;

namespace Northwind.Features.InnerJoin;

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

public class Vehicle
{
    public string InsuranceId { get; set; }

    public string OwnerId { get; set; }
    
    public string RegNo { get; set; }
    
    public string VehicleDetailsId { get; set; }
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

    public void Print()
    {
        Console.WriteLine("Vehicle details:\n");
        Console.WriteLine($"Registration Number: \t{this.RegNo}");
        Console.WriteLine($"Chassis Number: \t{this.ChassisNo}");
        Console.WriteLine($"Engine Number: \t\t{this.EngineNo}");
        Console.WriteLine($"Registration date: \t{this.Registration.ToShortDateString()}");
        Console.WriteLine($"Owner: \t\t\t{this.OwnerName}");
        Console.WriteLine($"Owner Address: \t\t{this.OwnerAddress}");
        Console.WriteLine($"Owner DOB: \t\t{this.OwnerDob.ToShortDateString()}");
        Console.WriteLine($"Insurance Company: \t{this.InsuranceCompanyName}");
        Console.WriteLine($"Insurance Address: \t{this.InsuranceCompanyAddress}");
        Console.WriteLine($"Insurance Expiration: \t{this.InsuranceExpiration}");
    }
}

