using Hotel.Models;
using Raven.Client.Documents.Indexes;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Indexes
{
    public class Rooms_ByInterval : AbstractMultiMapIndexCreationTask<Rooms_ByInterval.Entry>
    {
        public class Entry
        {
            public string Room { get; set; }

            public string Status { get; set; }

            public bool HasReservations { get; set; }
        }

        public Rooms_ByInterval()
        {
            AddMap<RoomStatus>(
                collection => from rs in collection
                         select new Entry
                         {
                             Room = rs.Room,
                             Status = rs.Status,
                             HasReservations = false
                         });

            // original version
            Reduce = results => from result in results
                                group result by result.Room
                                into g

                                select new Entry
                                {
                                    Room = g.Key,
                                    Status = String.Join(",", g.Select(x => x.Status).Distinct()),
                                    HasReservations = g.Any(x => x.Status == "ReadyForGuests")
                                };

            StoreAllFields(FieldStorage.Yes);

            OutputReduceToCollection = "IntervalRooms";
        }
    }
}
