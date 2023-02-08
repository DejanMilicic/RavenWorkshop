using System.Collections.Generic;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Indexes.DynamicFields.ProductAttributes
{
    public class Product
    {
        public string Id { get; set; }

        public string SKU { get; set; }

        // The KEYS in the Attributes dictionary will be dynamically indexed 
        // Attributes added to this object after index creation time will also get indexed
        public Dictionary<string, object> Attributes { get; set; }
    }

    public static class Seed
    {
        public static void Execute(IDocumentSession session)
        {
            session.Store(
                new Product
                {
                    SKU = "sneakers-1",
                    Attributes = new Dictionary<string, object>
                    {
                        { "Category", "Sneakers" },
                        { "Size", 39 },
                        { "Color", "red" }
                    }
                });

            session.Store(
                new Product
                {
                    SKU = "shoes-1",
                    Attributes = new Dictionary<string, object>
                    {
                        { "Category", "Shoes" },
                        { "Size", 40 },
                        { "Material", "Leather" },
                        { "Waterproof", true }
                    }
                });
        }
    }
}
