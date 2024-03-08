using System;
using System.Collections.Generic;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Indexes.JavaScript;

public class ProductSales_ByMonth : AbstractJavaScriptIndexCreationTask
{
    public class Result
    {
        public string Product { get; set; }
        public DateTime Month { get; set; }
        public int Count { get; set; }
        public decimal Total { get; set; }
    }

    public ProductSales_ByMonth()
    {
        Maps = new HashSet<string>
            {
                @"map('orders', function(order) {
                           var res = [];
                           order.Lines.forEach(l => {
                               res.push({
                                   Product: l.Product,
                                   Month: new Date( (new Date(order.OrderedAt)).getFullYear(),(new Date(order.OrderedAt)).getMonth(),1),
                                   Count: 1,
                                   Total: (l.Quantity * l.PricePerUnit) * (1- l.Discount)
                               })
                           });
                           return res;
                    })"
            };

        Reduce = @"groupBy(x => ({Product: x.Product, Month: x.Month}))
                    .aggregate(g => {
                        return {
                            Product: g.key.Product,
                            Month: g.key.Month,
                            Count: g.values.reduce((sum, x) => x.Count + sum, 0),
                            Total: g.values.reduce((sum, x) => x.Total + sum, 0)
                        }
                })";

        OutputReduceToCollection = "MonthlyProductSales";
        PatternReferencesCollectionName = "MonthlyProductSales/References";
        PatternForOutputReduceToCollectionReferences = "sales/monthly/{Month}";
    }
}