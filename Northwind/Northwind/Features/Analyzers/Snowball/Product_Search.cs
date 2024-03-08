using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Analyzers.Snowball;

public class Product_Search : AbstractIndexCreationTask<Product>
{
    public Product_Search()
    {
        Map = products => 
                  from product in products
                  select new
                  {
                      product.Name
                  };

        Index(x => x.Name, FieldIndexing.Search);
        Analyzers.Add(x => x.Name, "SnowballAnalyzer");
    }
}
