using System;
using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.ArtificialDocuments.DailyProductSales;

public class Product_Sales_ByDate : AbstractIndexCreationTask<Order, DailyProductSale>
{
    public Product_Sales_ByDate()
    {
        Map = orders => from order in orders
                        from line in order.Lines
                        select new DailyProductSale
                        {
                            Product = line.Product,
                            Date = new DateTime(order.OrderedAt.Year, order.OrderedAt.Month, order.OrderedAt.Day),
                            Count = 1,
                            Total = line.Quantity * line.PricePerUnit * (1 - line.Discount)
                        };

        Reduce = results => from result in results
                            group result by new { result.Product, result.Date } into g
                            select new DailyProductSale
                            {
                                Product = g.Key.Product,
                                Date = g.Key.Date,
                                Count = g.Sum(x => x.Count),
                                Total = g.Sum(x => x.Total)
                            };

        OutputReduceToCollection = "DailyProductSales";
        PatternReferencesCollectionName = "DailyProductSales/References";
        PatternForOutputReduceToCollectionReferences = x => $"sales/daily/{x.Date:yyyy-MM-dd}";
    }
}
