using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.AdditionalSources.AveragePrice
{
    public class MyProductPriceHistory
    {
        public string Id { get; set; }
        
        public string ProductId { get; set; }

        public List<PriceEntry> Entries { get; set; } = new List<PriceEntry>();
    }

    public class PriceEntry
    {
        public DateTime Timestamp { get; set; }

        public decimal Price { get; set; }
    }
}
