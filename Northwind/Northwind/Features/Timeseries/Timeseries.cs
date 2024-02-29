using System;
using System.Collections.Generic;
using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents.Session;
using Raven.Client.Documents.Session.TimeSeries;
using static System.Formats.Asn1.AsnWriter;
using static Northwind.Models.Entity.Company;

namespace Northwind.Features.Timeseries
{
    public static class Timeseries
    {
        // There is no need to explicitly create Timeseries
        // appending a value will automatically create it if it does not already exist
        public static void AppendNative()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            ISessionDocumentTimeSeries ts = session.TimeSeriesFor("shippers/3-A", "Likes");
            List<double> values = new List<double> { 1.1, 2.15 };
            ts.Append(DateTime.UtcNow, values);

            session.SaveChanges();
        }

        public static void AppendNamed()
        {
            DocumentStoreHolder.Store.TimeSeries.Register<Shipper, StockPrice>();

            using var session = DocumentStoreHolder.Store.OpenSession();

            session.TimeSeriesFor<StockPrice>("shippers/1-A")
                .Append(DateTime.UtcNow, new() { Open = 515.267, Close = 580.1, High = 613.44, Low = 499.99, Volume = 5487 });

            session.SaveChanges();
        }

        public static void Insert()
        {
            // todo : add timer
            // todo : correct dates (year 3000)
            // todo : index this data and run queries

            using var bulk = DocumentStoreHolder.Store.BulkInsert();
            var random = new Random();
            var time = DateTime.Today.AddDays(-30);

            for (int i = 1; i < 10; i++)
            {
                using var ts = bulk.TimeSeriesFor($"employees/{i}-A", "Audits");

                for (int j = 0; j < 100_000; j++)
                {
                    time = time.AddHours(random.Next(6, 24));
                    ts.Append(time, 1, "Login");

                    time = time.AddSeconds(random.Next(600, 10_000));
                    ts.Append(time, 0, "Logout");
                }
            }
        }

        public static void Query()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();
            TimeSeriesEntry[] val = session
                .TimeSeriesFor("employees/9-A", "HeartRates")
                .Get();

            Console.WriteLine(val[0].Values[0]);
            Console.WriteLine(val[1].Values[0]);
            Console.WriteLine(val[2].Values[0]);
        }

        public static void QueryStockPrices()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var stocks = 
                session
                .TimeSeriesFor<StockPrice>("companies/55-A")
                .Get(from: new DateTime(2020, 1, 1), to: new DateTime(2020, 6, 30))
                .GroupBy(g => g.Timestamp.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Min = g.Min(x => x.Value.Close),
                    Max = g.Max(x => x.Value.Close)
                })
                .ToList();

            foreach (var item in stocks)
            {
                Console.WriteLine($"{item.Month} \t {item.Min} \t {item.Max}");
            }

            /*
            from Companies
            where id() = 'companies/55-A'
            select timeseries(
                from StockPrices
                between '2020-01-01' and '2020-06-30'
                group by '1 month'
                select min(), max()
            ) as StockPrices
            */
        }
    }
}

// todo : write some indexes for timeseries