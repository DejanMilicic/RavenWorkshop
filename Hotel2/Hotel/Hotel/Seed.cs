using Hotel.Models;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Session;

namespace Hotel
{
    internal static class Seed
    {
        internal static void Do()
        {
            var stats2 = DocumentStoreHolder.Store.Maintenance.Send(new GetCollectionStatisticsOperation());
            bool hasDocuments = stats2.Collections.Count(x => x.Value > 0 && !x.Key.StartsWith("@")) > 0;
            if (hasDocuments) return;
            
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
                Type = "apart",
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

            session.Store(new Room
            {
                Id = "Rooms/104",
                Beds = 1,
                Type = "studio",
                InUse = false
            });
        }

        private static void SeedReservations(IDocumentSession session)
        {
            session.Store(new Reservation
            {
                Room = "Rooms/101",
                Start = new DateTime(2022, 1, 1, 14, 0, 0),
                End = new DateTime(2022, 1, 2, 23, 10, 0),
                GuestsIn = new DateTime(2022, 1, 1, 17, 0, 0),
                GuestsOut = null
            });
        }
    }
}
