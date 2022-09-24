namespace Hotel.Models
{
    internal class Bid
    {
        public string Id { get; set; }

        public string Room { get; set; }

        public bool Vip { get; set; }

        public DateOnly Start { get; set; }

        public DateOnly End { get; set; }
    }
}
