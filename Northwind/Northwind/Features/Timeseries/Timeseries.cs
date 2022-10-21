using System;
using System.Collections.Generic;
using Raven.Client.Documents.Session;
using Raven.Client.Documents.Session.TimeSeries;

namespace Northwind.Features.Timeseries
{
    public static class Timeseries
    {
        // There is no need to explicitly create Timeseries
        // appending a value will automatically create it if it does not already exist
        public static void Do()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            ISessionDocumentTimeSeries ts = session.TimeSeriesFor("shippers/3-A", "Likes");
            List<double> values = new List<double> { 1.1, 2.15 };
            ts.Append(DateTime.UtcNow, values);

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
    }
}
