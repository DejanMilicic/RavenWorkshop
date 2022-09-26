using System.Collections.Generic;

namespace Northwind.Features.Facets2.Models
{
    public class Product
    {
        public string Id { get; set; }

        public string Title { get; set; }
        
        public string Brand { get; set; }
        
        public IList<ProductOption> Options { get; set; }
    }

    public class ProductOption
    {
        public int OptionId { get; set; }

        public bool Available { get; set; }

        public decimal Price { get; set; }

        public IList<ProductOptionAttribute> Attributes { get; set; }
    }

    public class ProductOptionAttribute
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}