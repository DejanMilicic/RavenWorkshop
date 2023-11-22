using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Indexes.MultipleResolutions;

public class Orders_ByMultipleResolutions : AbstractIndexCreationTask<Order, Orders_ByMultipleResolutions.Entry>
{
    public class Entry
    {
        public string FormattedDate { get; set; }

        public string Format { get; set; }

        public string Company { get; set; }
        
        public int Count { get; set; }
    }

    public Orders_ByMultipleResolutions()
    {
        Map = orders =>
            from order in orders
            from r in new[] { "yyyy-MM-dd", "yyyy-MM", "yyyy" }
            select new Entry
            {
                FormattedDate = order.OrderedAt.ToString(r),
                //Date = DateTime.ParseExact(order.OrderedAt.ToString(r), r, CultureInfo.InvariantCulture),
                Format = r,
                Company = order.Company,
                Count = 1
            };

        Reduce = results => from result in results
            group result by new { result.FormattedDate, result.Format, result.Company }
            into g
            select new
            {
                g.Key.FormattedDate,
                g.Key.Format,
                g.Key.Company,
                Count = g.Sum(x => x.Count)
            };
    }
}

