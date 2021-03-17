using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;

namespace Northwind.Features.Timeseries
{
    public class Timeseries
    {
        public void Do()
        {
            using var bulk = DocumentStoreHolder.Store.BulkInsert();
            var random = new Random();
            var time = DateTime.Today.AddDays(-30);

            for (int i = 1; i < 10; i++)
            {
                using var ts = bulk.TimeSeriesFor($"employees/{i}-A", "Audits");

                for (int j = 0; j < 10_000; j++)
                {
                    time = time.AddHours(random.Next(6, 24));
                    ts.Append(time, 1, "Login");
                    
                    time = time.AddSeconds(random.Next(600, 10_000));
                    ts.Append(time, 0, "Logout");
                }
            }
        }
    }
}
