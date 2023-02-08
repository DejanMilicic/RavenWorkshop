using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Indexes.DynamicFields.ProductAttributes
{
    public class Entry
    {
        public Dictionary<string, object> Attributes;
    }

    public class Products_ByAttribute_Typed : AbstractIndexCreationTask<Product>
    {
        public Products_ByAttribute_Typed()
        {
            Map = products => from p in products
                select new
                {
                    _ = p.Attributes.Select(item => CreateField("Attributes_" + item.Key, item.Value))
                };
        }
    }
}
