using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.IO;
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
                    Timeframe = "Month",
                    Timestamp = $"{sr.Timestamp.Year}-{sr.Timestamp.Month.ToString("D2")}",
                    Receivables = sr.RecievedAmount,
                    Payables = sr.PaidAmount
                }
            );

        AddMap<SalesRecord>(
            salesRecords => from sr in salesRecords
                select new Entry
                {
                    User = sr.User,
                    Timeframe = "Week",
                    Timestamp = $"{sr.Timestamp.Year}-{DateHelper.GetIso8601WeekNumber(sr.Timestamp).ToString("D2")}",
                    Receivables = sr.RecievedAmount,
                    Payables = sr.PaidAmount
                }
            );


        AddMap<SalesRecord>(
            salesRecords => from sr in salesRecords
                select new Entry
                {
                    User = sr.User,
                    Timeframe = "Quarter",
                    Timestamp = $"{sr.Timestamp.Year}-{DateHelper.GetQuarterNumber(sr.Timestamp)}",
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

        AdditionalSources = new Dictionary<string, string>
        {
            ["DateHelper.cs"] =
                File.ReadAllText(Path.Combine(new[]
                    { AppContext.BaseDirectory, "..", "..", "..", "Features", "Indexes", "SalesStats", "DateHelper.cs" }))
        };
    }
}
