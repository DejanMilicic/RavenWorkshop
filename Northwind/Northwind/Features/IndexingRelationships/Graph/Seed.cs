using System;

namespace Northwind.Features.IndexingRelationships.Graph
{
    public static class Seed
    {
        public static void Numbers()
        {
            using var session = GraphStoreHolder.Store.OpenSession();

            session.Store(new Flight
            {
                Id = "London",
                To = new[] { "Paris", "Stockholm" }
            });

            session.Store(new Flight
            {
                Id = "Paris",
                To = new[] { "Istanbul" }
            });

            session.Store(new Flight
            {
                Id = "Stockholm",
                To = Array.Empty<string>()
            });

            session.Store(new Flight
            {
                Id = "Istanbul",
                To = Array.Empty<string>()
            });

            session.SaveChanges();
        }
    }
}
