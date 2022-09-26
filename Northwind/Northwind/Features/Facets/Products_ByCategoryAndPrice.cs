using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Facets
{
    internal class Products_ByCategoryAndPrice : AbstractIndexCreationTask<Product, Products_ByCategoryAndPrice.Entry>
    {
        public class Entry
        {
            public string CategoryName { get; set; }

            public decimal PricePerUnit { get; set; }
        }

        public Products_ByCategoryAndPrice()
        {
            Map = products => from product in products
                              select new Entry
                              {
                                  CategoryName = LoadDocument<Category>(product.Category).Name,
                                  PricePerUnit = product.PricePerUnit
                              };
        }
    }
}
