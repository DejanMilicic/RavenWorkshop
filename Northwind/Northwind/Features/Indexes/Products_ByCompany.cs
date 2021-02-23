using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace Northwind.Features.Indexes
{
    public class Products_ByCompany : AbstractIndexCreationTask<Order, Products_ByCompany.Entry>
    {
        public class Entry
        {
            public string Company { get; set; }

            public string Product { get; set; }

            public string[] Query { get; set; }
        }

        public Products_ByCompany()
        {
            Map = orders => from order in orders
                            from orderLine in order.Lines
                            let product = LoadDocument<Product>(orderLine.Product)
                            select new Entry
                            {
                                Company = order.Company,
                                Product = product.Id,
                                Query = new []
                                {
                                    product.Name, 
                                    product.QuantityPerUnit, 
                                    order.ShipTo.City
                                }
                            };

            Index(x => x.Query, FieldIndexing.Search);
            Stores.Add(x => x.Company, FieldStorage.Yes);
            Stores.Add(x => x.Product, FieldStorage.Yes);
        }
    }
}
