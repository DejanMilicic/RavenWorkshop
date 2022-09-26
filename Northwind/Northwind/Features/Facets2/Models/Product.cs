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
}