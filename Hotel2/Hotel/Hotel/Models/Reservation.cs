namespace Hotel.Models
{
    internal class Reservation
    {
        public string Id { get; set; }

        public string Room { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public DateTime? GuestsIn { get; set; } // moment when guests enter the room

        public DateTime? GuestsOut { get; set; } // moment when guests exit the room
    }
}
