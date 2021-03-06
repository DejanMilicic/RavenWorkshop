using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace Northwind.Features.Indexes
{
    public class Orders_Search : AbstractIndexCreationTask<Order, Orders_Search.Entry>
    {
        public class Entry
        {
            public string[] Query { get; set; }
        }

        public Orders_Search()
        {
            Map = orders => from order in orders
                            from orderLine in order.Lines
                            select new Entry
                            {
                                Query = new []
                                {
                                    orderLine.ProductName
                                }
                            };

            Index(x => x.Query, FieldIndexing.Search);
        }
    }
}
