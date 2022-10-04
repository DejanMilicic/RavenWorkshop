using Raven.Client.Documents.Indexes;
using Hotel.Models;
using System;
using Raven.Client.Documents.Indexes.Analysis;

namespace Hotel.Indexes
{
    public class Reservations_ByHour : AbstractMultiMapIndexCreationTask<Entry>
    {


        public Reservations_ByHour()
        {
            // seed all rooms
            AddMap<Room>(
                rooms => from room in rooms
                         let start = new DateTime(2021, 12, 31, 0, 0, 0, DateTimeKind.Utc)
                         let end = new DateTime(2022, 12, 31, 0, 0, 0, DateTimeKind.Utc)
                         let ts = TimeSpan.FromHours(Math.Ceiling((end - start).TotalHours)) // round hours up
                         from hour in Enumerable.Range(0, (int)ts.TotalHours)

                         select new Entry
                         {
                             Room = room.Id,
                             IntervalStart = start.AddHours(hour),
                             IntervalEnd = start.AddHours(hour).AddHours(1),
                             Status = room.InUse ? "Available" : "NotInUse"
                         });

            // process reservation
            AddMap<Reservation>(
                reservations => from r in reservations

                                let ts = TimeSpan.FromHours(Math.Ceiling((r.End - r.Start).TotalHours)) // round hours up
                                from hour in Enumerable.Range(0, (int)ts.TotalHours)
                                let hourStart = r.Start.AddHours(hour)
                                select new Entry
                                {
                                    Room = r.Room,
                                    IntervalStart = hourStart,
                                    IntervalEnd = hourStart.AddHours(1),
                                    Status = "ReadyForGuests"
                                });

            // process guests who entered room and they are still in it
            AddMap<Reservation>(
                reservations => from r in reservations
                                where r.GuestsIn != null && r.GuestsOut == null

                                let ts = TimeSpan.FromHours(Math.Ceiling((r.End - r.GuestsIn.Value).TotalHours)) // round hours up
                                from hour in Enumerable.Range(0, (int)ts.TotalHours)
                                let hourStart = r.GuestsIn.Value.AddHours(hour)
                                select new Entry
                                {
                                    Room = r.Room,
                                    IntervalStart = hourStart,
                                    IntervalEnd = hourStart.AddHours(1),
                                    Status = "GuestsIn"
                                });

            // process guests who used room and left it
            AddMap<Reservation>(reservations => from r in reservations
                                                where r.GuestsIn != null && r.GuestsOut != null

                                                let ts = TimeSpan.FromHours(Math.Ceiling((r.GuestsOut.Value - r.GuestsIn.Value).TotalHours)) // round hours up
                                                from hour in Enumerable.Range(0, (int)ts.TotalHours)
                                                let hourStart = r.GuestsIn.Value.AddHours(hour)
                                                select new Entry
                                                {
                                                    Room = r.Room,
                                                    IntervalStart = hourStart,
                                                    IntervalEnd = hourStart.AddHours(1),
                                                    Status = "GuestsIn"
                                                });

            Reduce = results => from result in results
                                group result by new { result.Room, result.IntervalStart, result.IntervalEnd }
                                into g

                                select new Entry
                                {
                                    Room = g.Key.Room,
                                    IntervalStart = g.Key.IntervalStart,
                                    IntervalEnd = g.Key.IntervalEnd,
                                    //Status = ComputeStatus.Do(String.Join(",", g.Select(x => x.Status).Distinct()))
                                    Status = ComputeStatus.Do(g.Select(x => x.Status).Distinct())
                                };

            AdditionalSources = new Dictionary<string, string>
            {
                ["ComputeStatus.cs"] =
                    File.ReadAllText(Path.Combine(new[]
                        { AppContext.BaseDirectory, "..", "..", "..", "Indexes", "ComputeStatus.cs" }))
            };

            //AdditionalAssembly.FromRuntime("System.Collections.Generic", usings: new HashSet<string> { "System.Collections.Generic" });

            StoreAllFields(FieldStorage.Yes);

            OutputReduceToCollection = "RoomStatuses";
        }
    }
}
