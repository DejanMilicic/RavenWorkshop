using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

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
        public static void Seed()
        {
            var session = ReservationsStoreHolder.Store.OpenSession();

            if (session.Query<RoomDate>().Count() == 0)
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
                            User = "Jane Doe",
                            From = new DateTime(2022, 4, 1, 10, 30, 0),
                            To = new DateTime(2022, 4, 1, 12, 00, 0),
                        },

                        new RoomDate.Reservation
                        {
                            User = "John Doe",
                            From = new DateTime(2022, 4, 1, 14, 00, 0),
                            To = new DateTime(2022, 4, 1, 16, 00, 0),
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
            public DateTime From { get; set; }

            public DateTime To { get; set; }
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
                        From = x.Item1,
                        To = x.Item2
                    })
            );

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
