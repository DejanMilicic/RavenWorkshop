using Raven.Client.Documents.Indexes;
using Hotel.Models;

namespace Hotel.Indexes
{
    internal class Bids_ByDay : AbstractIndexCreationTask<Bid, Bids_ByDay.Entry>
    {
        internal class Entry
        {
            public DateOnly Day { get; set; }

            public int TotalBids { get; set; }

            public int VipBids { get; set; }

            public List<string> Rooms { get; set; }
        }

        public Bids_ByDay()
        {
            Map = bids => from bid in bids
                let days = bid.End.DayNumber - bid.Start.DayNumber + 1
                from day in Enumerable.Range(0, days)
                let d = bid.Start.AddDays(day)
                select new Entry
                {
                    Day = new DateOnly(d.Year, d.Month, d.Day),
                    TotalBids = 1,
                    VipBids = bid.Vip ? 1 : 0,
                    Rooms = new List<string> { bid.Room }
                };

            Reduce = results => from result in results
                group result by new
                {
                    result.Day
                }
                into g
                select new Entry
                {
                    Day = g.Key.Day,
                    TotalBids = g.Sum(x => x.TotalBids),
                    VipBids = g.Sum(x => x.VipBids),
                    Rooms = g.SelectMany(x => x.Rooms).Distinct().ToList()
                };

            StoreAllFields(FieldStorage.Yes);
        }
    }
}
