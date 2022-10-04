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

            using var session = DocumentStoreHolder.Store.OpenSession();

            //DateOnly jan1 = new DateOnly(2022, 1, 1);
            //DateOnly jan31 = new DateOnly(2022, 1, 31);

            List<Room> rooms = session.Query<Room>().ToList();
            DateTime moment = new DateTime(2022, 1, 1, 14, 0, 0);

            PrintRoomsAvailability(rooms, moment);

            // TODO:
            // 1. sorting by RoomNumber, NumberOfBeds, TypeOfRoom, Status { NotAvailable, Available, GuestsIn, ReadyForGuests, ReservationsInFuture}
            // 2. paging
            
            void PrintRoomsAvailability(List<Room> rooms, DateTime moment)
            {
                Console.WriteLine("Rooms\n");
                Console.WriteLine("Room\t\tBeds\tType\t\tStatus\n");


                foreach (Room room in rooms)
                {
                    // NotAvailable, Available, GuestsIn, ReadyForGuests, ReservationsInFuture
                    string status = "TODO";

                    if (!room.InUse)
                        status = "Not Available";
                    else
                    {
                        // todo: process other statuses
                    }

                    Console.WriteLine($"{room.Id}\t{room.Beds}\t{room.Type} \t\tTODO");
                }
            }



            /*


            Show_Room101_January2022_Reservations();
            Console.WriteLine();
            Show_Free_Days_Room101_January2022();
            Console.WriteLine();
            Show_Most_Bidded_Rooms();
            Console.WriteLine();
            Show_Room101_Most_Bidded_Days();
            Console.WriteLine();
            Show_Most_Popular_Days();
            */

            //void Show_Room101_January2022_Reservations()
            //{
            //    string room = "101";

            //    List<Reservations_ByHour.Entry> room101_jan2022_reservations = session
            //        .Query<Reservation, Reservations_ByHour>()
            //        .ProjectInto<Reservations_ByHour.Entry>()
            //        .Where(r => (r.Room == room) && (r.Day >= jan1 && r.Day <= jan31))
            //        .ToList();

            //    Console.WriteLine($"Reservations for Room {room} between {jan1} and {jan31}");
            //    foreach (var r in room101_jan2022_reservations)
            //    {
            //        Console.WriteLine($"Room {r.Room} - {r.Day}");
            //    }
            //}

            //void Show_Free_Days_Room101_January2022()
            //{
            //    string room = "101";

            //    var room101_jan2022_reservations = session
            //        .Query<Reservation, Reservations_ByHour>()
            //        .ProjectInto<Reservations_ByHour.Entry>()
            //        .Where(r => (r.Room == room) && (r.Day >= jan1 && r.Day <= jan31))
            //        .ToList()
            //        .Select(r => r.Day)
            //        ;

            //    List<DateOnly> freeDaysJan2022 = Enumerable.Range(0, 31).Select(d => new DateOnly(2022, 1, 1 + d))
            //        .Where(day => !room101_jan2022_reservations.Contains(day))
            //        .ToList();

            //    Console.WriteLine($"Free days for Room {room} between {jan1} and {jan31}");
            //    foreach (var day in freeDaysJan2022)
            //    {
            //        Console.WriteLine($"Date: {day}");
            //    }
            //}

            //void Show_Most_Bidded_Rooms()
            //{
            //    List<Bids_ByRoom.Entry> topBids = session
            //        .Query<Bid, Bids_ByRoom>()
            //        .ProjectInto<Bids_ByRoom.Entry>()
            //        .OrderByDescending(x => x.TotalBids)
            //        .ToList();

            //    Console.WriteLine($"Rooms with most bids");
            //    foreach (var entry in topBids)
            //    {
            //        Console.WriteLine($"Room: {entry.Room} - Bids: {entry.TotalBids}");
            //    }
            //}

            //void Show_Room101_Most_Bidded_Days()
            //{
            //    string room = "101";

            //    List<Bids_ByDay_ByRoom.Entry> most_bidded_days = session
            //            .Query<Bid, Bids_ByDay_ByRoom>()
            //            .ProjectInto<Bids_ByDay_ByRoom.Entry>()
            //            .Where(r => r.Room == room)
            //            .OrderByDescending(x => x.TotalBids)
            //            .ToList();

            //    Console.WriteLine($"Most bidded days for Room {room}");
            //    foreach (var entry in most_bidded_days)
            //    {
            //        Console.WriteLine($"Date: {entry.Day} - Bids: {entry.TotalBids}");
            //    }
            //}

            //void Show_Most_Popular_Days()
            //{
            //    var most_bidded_days = session
            //        .Query<Bid, Bids_ByDay>()
            //        .ProjectInto<Bids_ByDay.Entry>()
            //        .OrderByDescending(x => x.TotalBids)
            //        .ToList();

            //    Console.WriteLine($"Most bidded days");
            //    foreach (var entry in most_bidded_days)
            //    {
            //        Console.WriteLine($"Date: {entry.Day} - Bids: {entry.TotalBids} - Rooms: {String.Join(", ", entry.Rooms)}");
            //    }
            //}
        }
    }
}