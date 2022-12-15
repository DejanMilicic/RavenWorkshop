using System;
using System.Collections.Generic;

namespace Northwind.Features.Fanout
{
    public class Sneaker
    {
        public string Id { get; set; }

        public string Model { get; set; }

        public string Company { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public List<PriceHistoryEntry> PriceHistory { get; set; } = new List<PriceHistoryEntry>();
    }

    public class PriceHistoryEntry
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public decimal Price { get; set; }
    }
}
