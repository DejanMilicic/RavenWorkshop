using System;
using CSharpVitamins;

namespace Northwind.Features.Records
{
    public record Receipt
    {
        public string Id { get; init; } = ShortGuid.Encode(Guid.NewGuid());

        public string Store { get; init; }

        public string Description { get; init; }

        public decimal Amount { get; init; }

        public DateTime PurchaseDate { get; init; }
    }

    public record Shipment(string Id, string UserId, string ShipmentId);
}
