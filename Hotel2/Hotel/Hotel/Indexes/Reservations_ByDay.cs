using Raven.Client.Documents.Indexes;
using Hotel.Models;

namespace Hotel.Indexes
{
    //internal class Reservations_ByDay : AbstractIndexCreationTask<Reservation, Reservations_ByDay.Entry>
    //{
    //    internal class Entry
    //    {
    //        public string Room { get; set; }

    //        public DateOnly Day { get; set; }
    //    }

    //    public Reservations_ByDay()
    //    {
    //        Map = reservations => from r in reservations
    //            let days = r.End.DayNumber - r.Start.DayNumber + 1
    //            from day in Enumerable.Range(0, days)
    //            let d = r.Start.AddDays(day)
    //            select new Entry
    //            {
    //                Room = r.Room,
    //                Day = new DateOnly(d.Year, d.Month, d.Day)
    //            };

    //        StoreAllFields(FieldStorage.Yes);
    //    }
    //}
}
