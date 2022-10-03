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
            SeedBids(session);
            
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
                Start = new DateOnly(2022, 1, 1),
                End = new DateOnly(2022, 1, 3),
                Status = "active"
            });

            session.Store(new Reservation
            {
                Room = "101",
                Start = new DateOnly(2022, 1, 31),
                End = new DateOnly(2022, 2, 1),
                Status = "active"
            });

            session.Store(new Reservation
            {
                Room = "102",
                Start = new DateOnly(2022, 1, 10),
                End = new DateOnly(2022, 1, 13),
                Status = "active"
            });

            session.Store(new Reservation
            {
                Room = "103",
                Start = new DateOnly(2022, 1, 31),
                End = new DateOnly(2022, 2, 2),
                Status = "active"
            });
        }

        private static void SeedBids(IDocumentSession session)
        {
            session.Store(new Bid
            {
                Room = "101",
                Start = new DateOnly(2022, 1, 10),
                End = new DateOnly(2022, 1, 11),
                Vip = true
            });

            session.Store(new Bid
            {
                Room = "101",
                Start = new DateOnly(2022, 1, 9),
                End = new DateOnly(2022, 1, 13),
                Vip = false
            });

            session.Store(new Bid
            {
                Room = "102",
                Start = new DateOnly(2022, 1, 9),
                End = new DateOnly(2022, 1, 13),
                Vip = false
            });

            session.Store(new Bid
            {
                Room = "103",
                Start = new DateOnly(2022, 1, 12),
                End = new DateOnly(2022, 1, 17),
                Vip = false
            });

        }
    }
}
