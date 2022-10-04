using Raven.Client.Documents.Indexes;
using Hotel.Models;
using System;
using Raven.Client.Documents.Indexes.Analysis;

namespace Hotel.Indexes
{
    internal class Reservations_ByHour : AbstractMultiMapIndexCreationTask<Reservations_ByHour.Entry>
    {
        internal class Entry
        {
            public string Room { get; set; }
            public DateTime IntervalStart { get; set; }
            public DateTime IntervalEnd { get; set; }
            public string Status { get; set; }
        }

        public Reservations_ByHour()
        {
            // process reservation
            AddMap<Reservation>(reservations => from r in reservations
                                  
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
            AddMap<Reservation>(reservations => from r in reservations 
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



            StoreAllFields(FieldStorage.Yes);
        }
    }
}
