using System;
using System.Collections.Generic;
using System.Linq;
using Northwind.Features.Projections;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Queries;

namespace Northwind.Features.Fanout
{
    public static class Fanout
    {
        public static void Demo()
        {
            var store = new DocumentStore
                {
                    Urls = new[] { "http://127.0.0.1:8080" },
                    Database = "Sneakers"
            }.Initialize();

            IndexCreation.CreateIndexes(new AbstractIndexCreationTask[] { new Sneakers_ByHistoricalPrices() }, store);

            // uncomment to seed
            // Seed.Data(store);

            using var session = store.OpenSession();

            // https://ravendb.net/docs/article-page/5.4/Csharp/client-api/session/querying/how-to-customize-query#projectionbehavior
            IRavenQueryable<Sneakers_ByHistoricalPrices.Entry> history = session
                .Query<Sneaker, Sneakers_ByHistoricalPrices>()
                .Customize(x => x.Projection(ProjectionBehavior.FromIndexOrThrow))
                .ProjectInto<Sneakers_ByHistoricalPrices.Entry>();

            Console.WriteLine("Products that were priced above 100 EUR historically");
            var res = history
                .Where(x => x.Price > 100 && x.Currency == "EUR")
                .ToList();
            foreach (Sneakers_ByHistoricalPrices.Entry entry in res)
                Console.WriteLine($"Model: {entry.Model}, {entry.Price} {entry.Currency} [{entry.From.ToShortDateString()} - {entry.To.ToShortDateString()}]");

            Console.WriteLine();

            Console.WriteLine("Periods when Terrex was between 70 EUR and 90 EUR ");
            var res2 = history
                .Where(x => x.Price >= 70 && x.Price <= 90 && x.Currency == "EUR")
                .ToList();
            foreach (Sneakers_ByHistoricalPrices.Entry entry in res2)
                Console.WriteLine($"Model: {entry.Model}, {entry.Price} {entry.Currency} [{entry.From.ToShortDateString()} - {entry.To.ToShortDateString()}]");
        }
    }
}
