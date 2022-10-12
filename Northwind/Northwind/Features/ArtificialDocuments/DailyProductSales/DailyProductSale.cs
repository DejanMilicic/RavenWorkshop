using System;

namespace Northwind.Features.ArtificialDocuments.DailyProductSales
{
    public class DailyProductSale
    {
        public string Product { get; set; }

        public DateTime Date { get; set; }

        public int Count { get; set; }

        public decimal Total { get; set; }
    }
}
