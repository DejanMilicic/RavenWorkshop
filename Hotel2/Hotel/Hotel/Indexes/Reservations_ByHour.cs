using Raven.Client.Documents.Indexes;
using Hotel.Models;
using System;

namespace Hotel.Indexes
{
    internal class Reservations_ByHour : AbstractIndexCreationTask<Reservation, Reservations_ByHour.Entry>
    {
        internal class Entry
        {
            public string Room { get; set; }

            public DateTime IntervalStart { get; set; }
            public DateTime IntervalEnd { get; set; }
        }

        public Reservations_ByHour()
        {
            Map = reservations => from r in reservations
                                  let ts = TimeSpan.FromHours(Math.Ceiling((r.End - r.Start).TotalHours)) // round hours up
                                  from hour in Enumerable.Range(0, (int)ts.TotalHours)
                                  let hourStart = r.Start.AddHours(hour)
                                  select new Entry
                                  {
                                      Room = r.Room,
                                      IntervalStart = hourStart,
                                      IntervalEnd = hourStart.AddHours(1)
                                  };

            StoreAllFields(FieldStorage.Yes);
        }
    }
}
