using Northwind.Features.ArtificialDocuments.DailyProductSales;
using Raven.Client.Documents.Indexes;
using System.Collections.Generic;
using System.Linq;

namespace Northwind.Features.Indexes.SalesStats;

public class Sales_Aggregations : AbstractMultiMapIndexCreationTask<Sales_Aggregations.Entry>
{
    public class Entry
    {
        public string Id { get; set; }

        public string User { get; set; }

        public string Timeframe { get; set; }

        public string Timestamp { get; set; }

        public decimal Receivables { get; set; }

        public decimal Payables { get; set; }
    }

    public Sales_Aggregations()
    {
        AddMap<SalesRecord>(
            salesRecords => from sr in salesRecords
                select new Entry
                {
                    User = sr.User,
                    Timeframe = "Year",
                    Timestamp = $"{sr.Timestamp.Year}",
                    Receivables = sr.RecievedAmount,
                    Payables = sr.PaidAmount
                }
            );

        AddMap<SalesRecord>(
            salesRecords => from sr in salesRecords
                            select new Entry
                            {
                                User = sr.User,
                                Timeframe = "YearMonth",
                                Timestamp = $"{sr.Timestamp.Year}-{sr.Timestamp.Month}",
                                Receivables = sr.RecievedAmount,
                                Payables = sr.PaidAmount
                            }
            );

        Reduce = results => 
            from r in results
            group r by new { r.User, r.Timeframe, r.Timestamp } into g
            select new
            {
                g.Key.User,
                g.Key.Timeframe,
                g.Key.Timestamp,
                Receivables = g.Sum(x => x.Receivables),
                Payables = g.Sum(x => x.Payables)
            };
    }
}
