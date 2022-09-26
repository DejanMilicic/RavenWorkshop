using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Queries.Facets;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Facets
{
    internal class MyFacetResult
    {
        public string FacetName { get; set; }
        public string FacetRange { get; set; }
        public decimal FacetCount { get; set; }
    }

    internal static class Facets
    {
        internal static void ProductsPriceRanges()
        {
            decimal range1 = 25;
            decimal range2 = 50;
            decimal range3 = 100;

            var categoryFacet = new Facet
            {
                FieldName = "CategoryName",
                DisplayFieldName = "Product Category"
            };

            var priceFacet = new RangeFacet<Product>
            {
                Ranges =
                {
                    product => product.PricePerUnit < range1,
                    product => product.PricePerUnit >= range1 && product.PricePerUnit < range2,
                    product => product.PricePerUnit >= range2 && product.PricePerUnit < range3,
                    product => product.PricePerUnit >= range3
                },
                DisplayFieldName = "Price per Unit"
            };

            Dictionary<string, FacetResult> queryResults;

            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                var query = session.Query<Product, Products_ByCategoryAndPrice>()
                    .AggregateBy(new List<FacetBase>
                    {
                        categoryFacet,
                        priceFacet
                    });

                queryResults = query.Execute();
            }

            List<MyFacetResult> facetsResults = new List<MyFacetResult>();

            foreach (var result in queryResults)
            {
                var facetName = result.Value.Name;

                foreach (var item in result.Value.Values)
                {
                    facetsResults.Add(new MyFacetResult()
                    {
                        FacetName = facetName, // i.e. PricePerUnit
                        FacetRange = item.Range, // i.e. PricePerUnit < 50
                        FacetCount = item.Count
                    });
                }
            }

            Console.WriteLine($"\nProducts by {categoryFacet.DisplayFieldName}\n");

            foreach (MyFacetResult f in facetsResults
                         .Where(f => f.FacetName == categoryFacet.DisplayFieldName)
                         .OrderByDescending(f => f.FacetCount)
                     )
            {
                Console.WriteLine($"{f.FacetRange}: {f.FacetCount}");
            }

            Console.WriteLine($"\nProducts by {priceFacet.DisplayFieldName}\n");

            foreach (MyFacetResult f in facetsResults
                         .Where(f => f.FacetName == priceFacet.DisplayFieldName)
                         .OrderByDescending(f => f.FacetCount)
                     )
            {
                Console.WriteLine($"{f.FacetRange}: {f.FacetCount}");
            }
        }
    }
}
