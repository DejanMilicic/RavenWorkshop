using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Indexes.DynamicFields.AllProperties;

public class Products_ByAllAttributes : AbstractIndexCreationTask<Product>
{
    public Products_ByAllAttributes()
    {
        Map = products => from product in products
            select new
            {
                // convert product to JSON and select all properties from it
                _ = AsJson(product).Select(x => CreateField(x.Key, x.Value))
            };
    }
}
