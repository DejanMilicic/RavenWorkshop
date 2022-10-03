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
                Id = "Rooms/101",
                Beds = 1,
                Type = "apartment",
                InUse = true
            });

            session.Store(new Room
            {
                Id = "Rooms/102",
                Beds = 3,
                Type = "suite",
                InUse = true

            });

            session.Store(new Room
            {
                Id = "Rooms/103",
                Beds = 1,
                Type = "studio",
                InUse = true
            });
        }

        private static void SeedReservations(IDocumentSession session)
        {
            session.Store(new Reservation
            {
                Room = "Rooms/101",
                Start = new DateTime(2022, 1, 1),
                End = new DateTime(2022, 1, 3),
                GuestsIn = new DateTime(2022, 1, 1),
                GuestsOut = new DateTime(2022, 1, 3)
            });

            //session.Store(new Reservation
            //{
            //    Room = "101",
            //    Start = new DateTime(2022, 1, 31),
            //    End = new DateTime(2022, 2, 1),
            //    Status = "active"
            //});

            //session.Store(new Reservation
            //{
            //    Room = "102",
            //    Start = new DateTime(2022, 1, 10),
            //    End = new DateTime(2022, 1, 13),
            //    Status = "active"
            //});

            //session.Store(new Reservation
            //{
            //    Room = "103",
            //    Start = new DateTime(2022, 1, 31),
            //    End = new DateTime(2022, 2, 2),
            //    Status = "active"
            //});
        }
    }
}
