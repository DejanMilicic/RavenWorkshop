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
                IsFollowedBy = "2"
            });

            session.Store(new Number
            {
                Id = "2",
                IsFollowedBy = "3"
            });

            session.Store(new Number
            {
                Id = "3",
                IsFollowedBy = ""
            });

            session.SaveChanges();
        }
    }
}
