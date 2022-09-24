using Hotel.Indexes;
using Hotel.Models;
using Raven.Client.Documents;

namespace Hotel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Seed.Do();

            var session = DocumentStoreHolder.Store.OpenSession();

            DateOnly jan1 = new DateOnly(2022, 1, 1);
            DateOnly jan31 = new DateOnly(2022, 1, 31);

            Show_Room101_January2022_Reservations();
            Console.WriteLine();
            Show_Free_Days_Room101_January2022();

            // todo : introduce multiple bids for rooms


            void Show_Room101_January2022_Reservations()
            {
                string room = "101";

                List<Reservations_ByDay.Entry> room101_jan2022_reservations = session
                    .Query<Reservation, Reservations_ByDay>()
                    .ProjectInto<Reservations_ByDay.Entry>()
                    .Where(r => (r.Room == room) && (r.Day >= jan1 && r.Day <= jan31))
                    .ToList();

                Console.WriteLine($"Reservations for Room {room} between {jan1} and {jan31}");
                foreach (var r in room101_jan2022_reservations)
                {
                    Console.WriteLine($"Room {r.Room} - {r.Day}");
                }
            }

            void Show_Free_Days_Room101_January2022()
            {
                string room = "101";

                var room101_jan2022_reservations = session
                    .Query<Reservation, Reservations_ByDay>()
                    .ProjectInto<Reservations_ByDay.Entry>()
                    .Where(r => (r.Room == room) && (r.Day >= jan1 && r.Day <= jan31))
                    .ToList()
                    .Select(r => r.Day)
                    ;

                List<DateOnly> freeDaysJan2022 = Enumerable.Range(0, 31).Select(d => new DateOnly(2022, 1, 1 + d))
                    .Where(day => !room101_jan2022_reservations.Contains(day))
                    .ToList();

                Console.WriteLine($"Free days for Room {room} between {jan1} and {jan31}");
                foreach (var day in freeDaysJan2022)
                {
                    Console.WriteLine($"Date: {day}");
                }
            }
        }
    }
}