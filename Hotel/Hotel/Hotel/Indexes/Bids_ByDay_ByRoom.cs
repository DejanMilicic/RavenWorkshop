using Raven.Client.Documents.Indexes;
using Hotel.Models;

namespace Hotel.Indexes
{
    internal class Bids_ByDay_ByRoom : AbstractIndexCreationTask<Bid, Bids_ByDay_ByRoom.Entry>
    {
        internal class Entry
        {
            public string Room { get; set; }

            public DateOnly Day { get; set; }

            public bool Vip { get; set; }

            public int TotalBids { get; set; }

            public int VipBids { get; set; }
        }

        public Bids_ByDay_ByRoom()
        {
            Map = bids => from bid in bids
                let days = bid.End.DayNumber - bid.Start.DayNumber + 1
                from day in Enumerable.Range(0, days)
                let d = bid.Start.AddDays(day)
                select new Entry
                {
                    Room = bid.Room,
                    Day = new DateOnly(d.Year, d.Month, d.Day),
                    Vip = bid.Vip,
                    TotalBids = 1,
                    VipBids = bid.Vip ? 1 : 0
                };

            Reduce = results => from result in results
                group result by new
                {
                    result.Room,
                    result.Day
                }
                into g
                select new Entry
                {
                    Room = g.Key.Room,
                    Day = g.Key.Day,
                    Vip = g.Any(x => x.Vip),
                    TotalBids = g.Sum(x => x.TotalBids),
                    VipBids = g.Sum(x => x.VipBids)
                };

            StoreAllFields(FieldStorage.Yes);
        }
    }
}
