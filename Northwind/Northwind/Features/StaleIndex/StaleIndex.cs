using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Features.Indexes;
using Northwind.Models.Entity;
using Raven.Client.Documents.Session;

namespace Northwind.Features.StaleIndex
{
    public static class StaleIndex
    {
        public static void IsIndexStale()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            List<Order> orders = session.Query<Orders_ByCompany.Entry, Orders_ByCompany>()
                .Statistics(out QueryStatistics stats)
                .Where(x => x.CompanyName == "companyName")
                .OfType<Order>()
                .ToList();

            Console.WriteLine($"Stale index: {stats.IsStale}");
        }
    }
}
