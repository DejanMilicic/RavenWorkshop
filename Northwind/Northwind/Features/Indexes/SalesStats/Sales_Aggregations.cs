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

        public decimal Balance { get; set; }

        public decimal Margin { get; set; }
    }

    public Sales_Aggregations()
    {
        AddMap<SalesRecord>(
            salesRecords => from sr in salesRecords
                where sr.LoadStatus == "Done"
                select new Entry
                {
                    User = sr.User,
                    Timeframe = "Year",
                    Timestamp = $"{sr.Timestamp.Year}",
                    Receivables = sr.RecievedAmount,
                    Payables = sr.PaidAmount,
                    Balance = 0,
                    Margin = 0
                }
            );

        AddMap<SalesRecord>(
            salesRecords => from sr in salesRecords
                where sr.LoadStatus == "Done"
                select new Entry
                {
                    User = sr.User,
                    Timeframe = "Month",
                    Timestamp = $"{sr.Timestamp.Year}-{sr.Timestamp.Month.ToString("D2")}",
                    Receivables = sr.RecievedAmount,
                    Payables = sr.PaidAmount,
                    Balance = 0,
                    Margin = 0
                }
            );

        AddMap<SalesRecord>(
            salesRecords => from sr in salesRecords
                where sr.LoadStatus == "Done"
                select new Entry
                {
                    User = sr.User,
                    Timeframe = "Week",
                    Timestamp = $"{sr.Timestamp.Year}-{Helper.GetIso8601WeekNumber(sr.Timestamp).ToString("D2")}",
                    Receivables = sr.RecievedAmount,
                    Payables = sr.PaidAmount,
                    Balance = 0,
                    Margin = 0
                }
            );


        AddMap<SalesRecord>(
            salesRecords => from sr in salesRecords
                where sr.LoadStatus == "Done"
                select new Entry
                {
                    User = sr.User,
                    Timeframe = "Quarter",
                    Timestamp = $"{sr.Timestamp.Year}-{Helper.GetQuarterNumber(sr.Timestamp)}",
                    Receivables = sr.RecievedAmount,
                    Payables = sr.PaidAmount,
                    Balance = 0,
                    Margin = 0
                }
            );

        Reduce = results => 
            from r in results
            group r by new { r.User, r.Timeframe, r.Timestamp } into g
            let receivables = g.Sum(x => x.Receivables)
            let payables = g.Sum(x => x.Payables)
            select new
            {
                g.Key.User,
                g.Key.Timeframe,
                g.Key.Timestamp,
                Receivables = receivables,
                Payables = payables,
                Balance = receivables - payables,
                Margin = Helper.CalculateMargin(receivables, payables)
            };

        AdditionalSources = new Dictionary<string, string>
        {
            ["Helper.cs"] =
                File.ReadAllText(Path.Combine(new[]
                    { AppContext.BaseDirectory, "..", "..", "..", "Features", "Indexes", "SalesStats", "Helper.cs" }))
        };
    }
}
