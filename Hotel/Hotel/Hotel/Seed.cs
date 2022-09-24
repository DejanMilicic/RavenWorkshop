using Hotel.Models;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Session;

namespace Hotel
{
    internal static class Seed
    {
        internal static void Do()
        {
            DatabaseStatistics? stats = DocumentStoreHolder.Store.Maintenance.Send(new GetStatisticsOperation());
            if (stats.CountOfDocuments > 0) return;

            using var session = DocumentStoreHolder.Store.OpenSession();

            SeedRooms(session);
            SeedReservations(session);
            
            session.SaveChanges();
        }

        private static void SeedRooms(IDocumentSession session)
        {
            session.Store(new Room
            {
                Id = "101"
            });

            session.Store(new Room
            {
                Id = "102"
            });

            session.Store(new Room
            {
                Id = "103"
            });
        }

        private static void SeedReservations(IDocumentSession session)
        {
            session.Store(new Reservation
            {
                Room = "101",
                Start = new DateOnly(2002, 1, 1),
                End = new DateOnly(2002, 1, 3),
                Status = "active"
            });
        }
    }
}
