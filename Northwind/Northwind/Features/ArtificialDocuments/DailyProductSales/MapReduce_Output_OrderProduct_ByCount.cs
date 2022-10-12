using Raven.Client.Documents.Indexes;
using System.Linq;

namespace Northwind.Features.ArtificialDocuments.DailyProductSales
{
    public class MapReduce_Output_OrderProduct_ByCount : AbstractIndexCreationTask<
        DailyProductSale, MapReduce_Output_OrderProduct_ByCount.Entry>
    {
        public class Entry
        {
            public string Product { get; set; }

            public int Count { get; set; }

            public int NumOrders { get; set; }
        }

        public MapReduce_Output_OrderProduct_ByCount()
        {
            Map = orders => from order in orders
                            let referenceDocuments = LoadDocument<OutputReduceToCollectionReference>(
                                $"sales/daily/{order.Date:yyyy-MM-dd}",
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
                                group r by new { r.Count, r.Product }
                            into g
                                select new
                                {
                                    g.Key.Product,
                                    g.Key.Count,
                                    NumOrders = g.Sum(x => x.NumOrders)
                                };
        }
    }
}
