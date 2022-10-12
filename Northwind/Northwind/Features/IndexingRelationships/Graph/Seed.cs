using System;

namespace Northwind.Features.IndexingRelationships.Graph
{
    public static class Seed
    {
        public static void Numbers()
        {
            using var session = GraphStoreHolder.Store.OpenSession();

            session.Store(new Number
            {
                Id = "1",
                FollowedBy = new[] { "2", "3" }
            });

            session.Store(new Number
            {
                Id = "2",
                FollowedBy = new[] { "3" }
            });

            session.Store(new Number
            {
                Id = "3",
                FollowedBy = Array.Empty<string>()
            });

            session.SaveChanges();
        }
    }
}
