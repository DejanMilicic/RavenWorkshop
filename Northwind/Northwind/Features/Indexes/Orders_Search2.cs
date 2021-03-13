using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;
using System.Linq;
using Raven.Client.Documents.Queries;

namespace Northwind.Features.Indexes
{
    public class Orders_Search2 : AbstractIndexCreationTask<Order, Orders_Search2.Entry>
    {
        public class Entry
        {
            public string[] Query { get; set; }
        }

        public Orders_Search2()
        {
            Map = orders => from order in orders
                            from orderLine in order.Lines

                            let product = LoadDocument<Product>(orderLine.Product)

                            select new Entry
                            {
                                Query = new []
                                {
                                    order.ShipTo.City,
                                    product.Name
                                }
                            };

            Index(x => x.Query, FieldIndexing.Search);
        }
    }
}
