using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Indexes.DynamicFields.ProductAttributes
{
    public class Products_ByAttribute : AbstractIndexCreationTask<Product>
    {
        public Products_ByAttribute()
        {
            Map = products => from p in products
                select new
                {
                    // Call 'CreateField' to generate dynamic-index-fields from the Attributes object keys
                    // Using '_' is just a convention. Any other string can be used instead of '_'

                    // The actual field name will be item.Key
                    // The actual field terms will be derived from item.Value
                    _ = p.Attributes.Select(item => CreateField(item.Key, item.Value))
                };
        }
    }
}
