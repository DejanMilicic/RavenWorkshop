using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session.TimeSeries;

namespace Northwind.Features.Timeseries
{
    public class Timeseries
    {
        public void Insert()
        {
            // todo : add timer
            // todo : correct dates (year 3000)

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

        public void Query()
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
