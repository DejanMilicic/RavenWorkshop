using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Fanout
{
    public static class Seed
    {
        public static void Data(IDocumentStore store)
        {
            using var session = store.OpenSession();

            Sneaker airmax = new Sneaker
            {
                Model = "Airmax",
                Company = "Nike",
                Price = 100,
                Currency = "EUR"
            };

            airmax.PriceHistory.Add(
                new PriceHistoryEntry
                {
                    Price = 90,
                    From = new DateTime(2022, 1, 1),
                    To = new DateTime(2022, 2, 15)
                });

            airmax.PriceHistory.Add(
                new PriceHistoryEntry
                {
                    Price = 98,
                    From = new DateTime(2022, 2, 16),
                    To = new DateTime(2022, 4, 3)
                });

            airmax.PriceHistory.Add(
                new PriceHistoryEntry
                {
                    Price = 102,
                    From = new DateTime(2022, 4, 4),
                    To = new DateTime(2022, 10, 1)
                });


            Sneaker terrex = new Sneaker
            {
                Model = "Terrex",
                Company = "Adidas",
                Price = 100,
                Currency = "EUR"
            };

            terrex.PriceHistory.Add(
                new PriceHistoryEntry
                {
                    Price = 50,
                    From = new DateTime(2022, 1, 1),
                    To = new DateTime(2022, 5, 10)
                });

            terrex.PriceHistory.Add(
                new PriceHistoryEntry
                {
                    Price = 97,
                    From = new DateTime(2022, 5, 11),
                    To = new DateTime(2022, 8, 31)
                });

            terrex.PriceHistory.Add(
                new PriceHistoryEntry
                {
                    Price = 101,
                    From = new DateTime(2022, 9, 1),
                    To = new DateTime(2022, 10, 1)
                });

            session.Store(airmax);
            session.Store(terrex);
            session.SaveChanges();
        }
    }
}
