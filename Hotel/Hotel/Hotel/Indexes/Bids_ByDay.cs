using Raven.Client.Documents.Indexes;
using Hotel.Models;

namespace Hotel.Indexes
{
    internal class Bids_ByDay : AbstractIndexCreationTask<Bid, Bids_ByDay.Entry>
    {
        internal class Entry
        {
            public string Room { get; set; }

            public DateOnly Day { get; set; }

            public bool Vip { get; set; }
        }

        public Bids_ByDay()
        {
            Map = bids => from bid in bids
                let days = bid.End.DayNumber - bid.Start.DayNumber + 1
                from day in Enumerable.Range(0, days)
                let d = bid.Start.AddDays(day)
                select new Entry
                {
                    Room = bid.Room,
                    Day = new DateOnly(d.Year, d.Month, d.Day),
                    Vip = bid.Vip
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
                    Vip = g.Any(x => x.Vip)
                    //Vip = g.SelectMany(x => x.Orders).ToList()
                };

            StoreAllFields(FieldStorage.Yes);
        }
    }
}
