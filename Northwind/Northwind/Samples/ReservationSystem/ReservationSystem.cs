using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session.Tokens;

namespace Northwind.Samples.ReservationSystem
{
    public static class ReservationsStoreHolder
    {
        private static readonly Lazy<IDocumentStore> LazyStore =
            new Lazy<IDocumentStore>(() =>
            {
                IDocumentStore store = new DocumentStore
                {
                    Urls = new[] { "http://127.0.0.1:8080" },
                    Database = "reservations"
                };

                store.Initialize();

                IndexCreation.CreateIndexes(new [] { new Rooms_ByEmptySlot() }, store);

                return store;
            });

        public static IDocumentStore Store => LazyStore.Value;
    }

    public static class ReservationSystem
    {
        public static void Query()
        {
            DateTime appointment = new DateTime(2022, 4, 1, 12, 0, 0);
            int length = 2;

            Console.WriteLine($"Customer: I need an appoint of {length}h on {appointment}");

            var session = ReservationsStoreHolder.Store.OpenSession();

            List<RoomDate> availableRooms = session.Query<Rooms_ByEmptySlot.Entry, Rooms_ByEmptySlot>()
                .Where(x => x.From <= appointment && appointment.AddHours(length) <= x.To)
                .OfType<RoomDate>()
                .ToList();

            if (availableRooms.Any())
            {
                Console.WriteLine("Company: We can offer you following rooms");
                foreach (RoomDate room in availableRooms)
                {
                    Console.WriteLine($"> {room.Name}");
                }
            }
            else
            {
                Console.WriteLine("Company: Unfortunately, we do not have any rooms available");
            }

            int minimumMinuteLength = 180;

            List<Rooms_ByEmptySlot.Entry> availableSlots = session.Query<Rooms_ByEmptySlot.Entry, Rooms_ByEmptySlot>()
                .Where(x => x.MinutesLength >= minimumMinuteLength)
                .ProjectInto<Rooms_ByEmptySlot.Entry>()
                .ToList();

            Console.WriteLine($"We can also offer following slots of at least {minimumMinuteLength} minutes");

            foreach (var slot in availableSlots)
            {
                Console.WriteLine($"> {slot.MinutesLength}m slot at room {slot.Id}");
            }
        }

        public static void Seed()
        {
            var session = ReservationsStoreHolder.Store.OpenSession();

            if (!session.Query<RoomDate>().Any())
            {
                session.Store(new RoomDate
                {
                    Name = "Office 1",
                    Open = new DateTime(2022, 4, 1, 9, 0, 0),
                    Close = new DateTime(2022, 4, 1, 20, 0, 0),
                    Reservations = new List<RoomDate.Reservation>
                    {
                        new RoomDate.Reservation
                        {
                            User = "Marco Polo",
                            From = new DateTime(2022, 4, 1, 9, 0, 0),
                            To = new DateTime(2022, 4, 1, 9, 30, 0),
                        },

                        new RoomDate.Reservation
                        {
                            User = "Jane Doe",
                            From = new DateTime(2022, 4, 1, 10, 30, 0),
                            To = new DateTime(2022, 4, 1, 12, 00, 0),
                        },

                        new RoomDate.Reservation
                        {
                            User = "John Doe",
                            From = new DateTime(2022, 4, 1, 14, 00, 0),
                            To = new DateTime(2022, 4, 1, 16, 00, 0),
                        },

                        new RoomDate.Reservation
                        {
                            User = "Magelan",
                            From = new DateTime(2022, 4, 1, 19, 45, 0),
                            To = new DateTime(2022, 4, 1, 20, 00, 0),
                        }
                    }
                });

                session.Store(new RoomDate
                {
                    Name = "Office 2",
                    Open = new DateTime(2022, 4, 1, 12, 0, 0),
                    Close = new DateTime(2022, 4, 1, 18, 0, 0)
                });
            }

            session.SaveChanges();
        }
    }

    public class Rooms_ByEmptySlot : AbstractIndexCreationTask<RoomDate, Rooms_ByEmptySlot.Entry>
    {
        public class Entry
        {
            public string Id { get; set; }

            public DateTime From { get; set; }

            public DateTime To { get; set; }

            public double MinutesLength { get; set; }
        }

        public Rooms_ByEmptySlot()
        {
            Map = rds => rds.SelectMany(
                rd => ExtractIntervals.Extract(
                        rd.Open, 
                        rd.Close, 
                        (rd.Reservations.Select(x => new Tuple<DateTime, DateTime>(x.From, x.To))).ToArray())
                    .Select(x => new Entry
                    {
                        Id = rd.Id,
                        From = x.Item1,
                        To = x.Item2,
                        MinutesLength = (x.Item2 - x.Item1).TotalMinutes
                    })
            );

            Stores.Add(x => x.MinutesLength, FieldStorage.Yes);

            AdditionalSources = new Dictionary<string, string>
            {
                ["ExtractIntervals.cs"] =
                   File.ReadAllText(Path.Combine(new[] { AppContext.BaseDirectory, "..", "..", "..", "Samples/ReservationSystem/ExtractIntervals.cs" }))
            };
        }
    }

    public class RoomDate
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime Open { get; set; }

        public DateTime Close { get; set; }

        public List<Reservation> Reservations { get; set; } = new List<Reservation>();

        public class Reservation
        {
            public DateTime From { get; set; }

            public DateTime To { get; set; }

            public string User { get; set; }
        }
    }
}
