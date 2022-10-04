using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Models
{
    internal class RoomStatus
    {
        public DateTimeOffset IntervalEnd { get; set; }
        public DateTimeOffset IntervalStart { get; set; }
        public string Room { get; set; }
        public string Status { get; set; }
    }
}
