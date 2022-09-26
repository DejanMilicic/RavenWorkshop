using System.Collections.Generic;

namespace Northwind.Features.Facets2.Models
{
    public class ProductOption
    {
        public int OptionId { get; set; }
        
        public bool Available { get; set; }
        
        public decimal Price { get; set; }
        
        public IList<ProductOptionAttribute> Attributes { get; set; }
    }
}