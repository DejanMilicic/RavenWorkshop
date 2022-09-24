namespace Hotel.Models
{
    internal class Reservation
    {
        public string Id { get; set; }

        public string Room { get; set; }

        public string Status { get; set; }
        
        public DateOnly Start { get; set; }
        
        public DateOnly End { get; set; }
    }
}
