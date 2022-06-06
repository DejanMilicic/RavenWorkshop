using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.AdditionalSources.AveragePrice
{
    public static class MyProductSeed
    {
        public static void Do()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            MyProduct banana = new MyProduct();
            banana.Name = "Banana";
            session.Store(banana);

            MyProductPriceHistory ph = new MyProductPriceHistory();
            ph.ProductId = banana.Id;
            ph.Entries.Add(new PriceEntry { Timestamp = new DateTime(2020, 1, 1), Price = 104.0m});
            ph.Entries.Add(new PriceEntry { Timestamp = new DateTime(2019, 1, 1), Price = 90.0m});
            ph.Entries.Add(new PriceEntry { Timestamp = new DateTime(2018, 1, 1), Price = 70.0m});
            session.Store(ph);

            session.SaveChanges();
        }
    }
}
