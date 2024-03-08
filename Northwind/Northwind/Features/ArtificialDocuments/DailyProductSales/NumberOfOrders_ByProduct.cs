using Raven.Client.Documents.Indexes;
using System.Collections.Generic;
using System.Linq;
using static Raven.Client.Constants;

namespace Northwind.Features.ArtificialDocuments.DailyProductSales;

// This is an example of a "recursive" map-reduce index
// It indexes the output documents of the index "Product_Sales_ByDate", using the reference documents.

public class NumberOfOrders_ByProduct : AbstractIndexCreationTask<DailyProductSale, NumberOfOrders_ByProduct.Entry>
{
    public class Entry
    {
        public string Product { get; set; }
        public int Count { get; set; }
        public int NumOrders { get; set; }
    }

    public class OutputReduceToCollectionReference
    {
        public string Id { get; set; }
        public List<string> ReduceOutputs { get; set; }
    }

    public NumberOfOrders_ByProduct()
    {
        Map = dailyProductSales => from sale in dailyProductSales
                        let referenceDocuments = LoadDocument<OutputReduceToCollectionReference>(
                            $"sales/daily/{sale.Date:yyyy-MM-dd}",
                            "DailyProductSales/References")

                        from refDoc in referenceDocuments.ReduceOutputs
                        let outputDoc = LoadDocument<DailyProductSale>(refDoc)
                        select new Entry
                        {
                            Product = outputDoc.Product,
                            Count = outputDoc.Count,
                            NumOrders = 1
                        };

        Reduce = results => from r in results
                        group r by new { r.Count, r.Product } into g
                        select new
                        {
                            g.Key.Product,
                            g.Key.Count,
                            NumOrders = g.Sum(x => x.NumOrders)
                        };
    }
}
