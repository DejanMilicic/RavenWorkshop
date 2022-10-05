using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Models
{
    internal class IntervalRoom
    {
        public string Room { get; set; }
        
        public string Status { get; set; }

        public bool HasFutureReservations { get; set; }

        public DateTimeOffset IntervalEnd { get; set; }

        public DateTimeOffset IntervalStart { get; set; }
    }
}
