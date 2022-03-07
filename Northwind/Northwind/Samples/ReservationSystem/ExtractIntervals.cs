using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Samples.ReservationSystem
{
    public static class ExtractIntervals
    {
        public static List<Tuple<DateTime, DateTime>> Extract(
            DateTime open,
            DateTime close,
            Tuple<DateTime, DateTime>[] reservations)
        {
            List<Tuple<DateTime, DateTime>> freeSlots = new List<Tuple<DateTime, DateTime>>();

            if (!reservations.Any())
            {
                freeSlots.Add(new(open, close));
            }
            else
            {
                freeSlots.Add(new(open, reservations.First().Item1));

                if (reservations.Count() > 1)
                {
                    for (int i = 0; i < reservations.Count()-1; i++)
                    {
                        freeSlots.Add(new(reservations.ElementAt(i).Item2, reservations.ElementAt(i+1).Item1));
                    }
                }

                freeSlots.Add(new(reservations.Last().Item2, close));
            }

            return freeSlots;
        }
    }
}
