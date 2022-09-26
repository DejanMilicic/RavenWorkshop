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

        private static void PrintProducts(List<Product> products)
        {
            Console.WriteLine("Products: ({0})", products.Count);
            //foreach (var product in products)
            //{
                
            //}
        }

        internal static void Execute()
        {
            var productsQueries = new ProductsQueries();

            Dictionary<string, IList<string>> filters;
            bool? availability;

            availability = null;
            filters = new Dictionary<string, IList<string>>();
            Console.WriteLine("Complete catalog");
            PrintProducts(productsQueries.GetProducts(availability, filters));
            PrintFacetResult(productsQueries.GetProductsFacets(availability, filters));

            availability = null;
            filters = new Dictionary<string, IList<string>> { { "Brand", new List<string> { "Nike" } } };
            Console.WriteLine("Catalog filtering: Brand='Nike'");
            PrintProducts(productsQueries.GetProducts(availability, filters));
            PrintFacetResult(productsQueries.GetProductsFacets(availability, filters));

            availability = true;
            filters = new Dictionary<string, IList<string>> { { "Size", new List<string> { "9" } } };
            Console.WriteLine("Catalog filtering: Availability=true AND Size='9'");
            PrintProducts(productsQueries.GetProducts(availability, filters));
            PrintFacetResult(productsQueries.GetProductsFacets(availability, filters));
        }
    }
}
