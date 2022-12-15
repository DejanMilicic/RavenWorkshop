using System;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace Northwind.Features.Fanout
{
    public class Sneakers_ByHistoricalPrices : AbstractIndexCreationTask<Sneaker, Sneakers_ByHistoricalPrices.Entry>
    {
        public class Entry
        {
            public string Model { get; set; }

            public DateTime From { get; set; }

            public DateTime To { get; set; }

            public decimal Price { get; set; }

            public string Currency { get; set; }
        }

        public Sneakers_ByHistoricalPrices()
        {
            Map = sneakers => from sneaker in sneakers
                from historicalPrice in sneaker.PriceHistory
                select new Entry
                {
                    Model = sneaker.Model,
                    From = historicalPrice.From,
                    To = historicalPrice.To,
                    Price = historicalPrice.Price,
                    Currency = sneaker.Currency
                };

            StoreAllFields(FieldStorage.Yes);
        }
    }
}
