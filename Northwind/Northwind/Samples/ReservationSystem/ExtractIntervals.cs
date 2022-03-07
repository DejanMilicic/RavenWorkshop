using System;
using System.Collections.Generic;
using System.Linq;

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
                if (reservations.First().Item1 > open)
                    freeSlots.Add(new(open, reservations.First().Item1));

                if (reservations.Count() > 1)
                {
                    for (int i = 0; i < reservations.Count()-1; i++)
                    {
                        //dynamic r = reservations;
                        //var element = r[i];

                        freeSlots.Add(new(reservations.ElementAt(i).Item2, reservations.ElementAt(i+1).Item1));
                    }
                }

                if (reservations.Last().Item2 < close)
                    freeSlots.Add(new(reservations.Last().Item2, close));
            }

            return freeSlots;
        }
    }
}
