using System;
using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;
using System.Collections.Generic;
using System.Linq;

namespace Northwind.Features.Indexes
{
    public class Orders_ByYearMonth : AbstractIndexCreationTask<Order, Orders_ByYearMonth.Entry>
    {
        public class Entry
        {
            public DateTime YearMonth { get; set; }

            public decimal Volume { get; set; }
        }

        public Orders_ByYearMonth()
        {
            Map = orders => from order in orders
                            select new Entry
                            {
                                YearMonth = new DateTime(order.OrderedAt.Year, order.OrderedAt.Month, 1),
                                Volume = order.Lines.Sum(l => (l.Quantity * l.PricePerUnit) * (1 - l.Discount))
                            };

            Reduce = results => from result in results
                                group result by new
                                {
                                    result.YearMonth
                                }
                    into g
                                select new Entry
                                {
                                    YearMonth = g.Key.YearMonth,
                                    Volume = g.Sum(x => x.Volume)
                                };
        }
    }
}

// from index 'Orders/ByYearMonth'
// order by Volume as long desc