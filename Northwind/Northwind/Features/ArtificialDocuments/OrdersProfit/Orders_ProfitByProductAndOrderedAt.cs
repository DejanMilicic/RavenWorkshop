using System;
using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.ArtificialDocuments.OrdersProfit
{
    public class Orders_ProfitByProductAndOrderedAt : AbstractIndexCreationTask<Order, Orders_ProfitByProductAndOrderedAt.Entry>
    {
        public class Entry
        {
            public string Product { get; set; }

            public DateTime OrderedAt { get; set; }
            
            public decimal Profit { get; set; }
        }

        public Orders_ProfitByProductAndOrderedAt()
        {
            Map = orders => from order in orders
                from line in order.Lines
                select new Entry
                {
                    Product = line.Product,
                    OrderedAt = order.OrderedAt,
                    Profit = line.Quantity * line.PricePerUnit * (1 - line.Discount)
                };

            Reduce = results => from r in results
                group r by new { r.OrderedAt, r.Product }
                into g
                select new Entry
                {
                    Product = g.Key.Product,
                    OrderedAt = g.Key.OrderedAt,
                    Profit = g.Sum(r => r.Profit)
                };

            OutputReduceToCollection = "Profits";
            PatternReferencesCollectionName = "Profits/References";
            PatternForOutputReduceToCollectionReferences = x => $"reports/daily/{x.OrderedAt:yyyy-MM-dd}";
        }
    }
}
