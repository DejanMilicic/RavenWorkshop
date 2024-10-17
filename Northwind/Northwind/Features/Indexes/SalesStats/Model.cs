using System;

namespace Northwind.Features.Indexes.SalesStats;

public class SalesRecord
{
    public string Id { get; set; }

    public string User { get; set; }

    public DateTime Timestamp { get; set; }

    public decimal RecievedAmount { get; set; }

    public decimal PaidAmount { get; set; }

    public string LoadStatus { get; set; }
}
