namespace Hotel.Models
{
    internal class Room
    {
        public string Id { get; set; } // room number

        public int Beds { get; set; } // 1, 2, 3

        public string Type { get; set; } // family, studio, suite, apartment

        public bool InUse { get; set; }
    }
}
