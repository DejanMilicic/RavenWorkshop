using System.Linq;
using Northwind.Features.Facets2.Models;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Facets2.Indexes
{
    public class Products_Options : AbstractIndexCreationTask<Product>
    {
        public Products_Options()
        {
            Map = products => from product in products
                from option in product.Options
                select new
                {
                    product.Title,
                    product.Brand,
                    Option_Available = option.Available,
                    _ = option.Attributes.Select(attribute => CreateField(attribute.Name, attribute.Value))
                };
        }
    }
}