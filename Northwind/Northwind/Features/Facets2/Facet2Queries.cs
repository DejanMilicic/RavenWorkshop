using Raven.Client.Documents.Queries.Facets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Features.Facets2.Models;

namespace Northwind.Features.Facets2
{
    internal static class Facet2Queries
    {
        private static void PrintFacetResult(IDictionary<string, FacetResult> facets)
        {
            foreach (var facetResult in facets)
            {
                Console.WriteLine("{0}: {1}", facetResult.Key,
                    string.Join(", ", facetResult.Value.Values.Select(v => $"{v.Range} ({v.Count})")));
            }

            Console.WriteLine();
        }

        internal static void Execute()
        {
            var productsQueries = new ProductsQueries();

            Console.WriteLine("Complete catalog");
            PrintFacetResult(productsQueries.GetProductsFacets(
                available: null, 
                filters: new Dictionary<string, IList<string>>()));

            Console.WriteLine("Catalog filtering: Brand='Nike'");
            PrintFacetResult(productsQueries.GetProductsFacets(
                available: null, 
                filters: new Dictionary<string, IList<string>> { { "Brand", new List<string> { "Nike" } } }));

            Console.WriteLine("Catalog filtering: Availability=true AND Size='9'");
            PrintFacetResult(productsQueries.GetProductsFacets(
                available: true,
                filters: new Dictionary<string, IList<string>> { { "Size", new List<string> { "9" } } }));
        }
    }
}
