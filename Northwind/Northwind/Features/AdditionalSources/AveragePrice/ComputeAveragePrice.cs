using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.AdditionalSources.AveragePrice
{
    // todo : create JS additional source example
    // todo : example of how to use NuGet package

    public static class ComputeAveragePrice
    {
        public static decimal FromPriceHistory(List<decimal> prices)
        {
            decimal total = 0;

            foreach (decimal price in prices)
            {
                total += price;
            }

            return total / prices.Count();
        }
    }
}
