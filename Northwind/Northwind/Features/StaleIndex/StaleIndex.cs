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
        // this method will display info if index Orders/ByCompany was stale in the moment of querying
        // try running it while this script is executing
        //
        // from Orders
        // update {
        //    for (var i=0; i<100; i++) {
        //        put("orders/", this)
        //    }
        // }
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
