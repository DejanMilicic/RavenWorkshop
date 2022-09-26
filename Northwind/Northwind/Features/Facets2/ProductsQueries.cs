using Northwind.Features.Facets2.Models;
using Raven.Client.Documents.Queries.Facets;
using Raven.Client.Documents.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Features.Facets2.Indexes;

namespace Northwind.Features.Facets2
{
    public class ProductsQueries
    {
        public List<Product> GetProducts(bool? available, IDictionary<string, IList<string>> filters)
        {
            using var session = FacetsStoreHolder.Store.OpenSession();

            var query = session.Advanced.DocumentQuery<Product, Products_Options>()
                .UsingDefaultOperator(QueryOperator.And)
                .Statistics(out var stats);
            
            if (available.HasValue)
                query.WhereLucene("Option_Available", available.ToString());

            foreach (var filter in filters)
            {
                query.WhereLucene(filter.Key, $"({string.Join(" OR ", filter.Value)})");
            }

            return query.ToList();
        }

        public Dictionary<string, FacetResult> GetProductsFacets(bool? available, IDictionary<string, IList<string>> filters)
        {
            using var session = FacetsStoreHolder.Store.OpenSession();

            var query = session.Advanced.DocumentQuery<Product, Products_Options>()
                .UsingDefaultOperator(QueryOperator.And)
                .Statistics(out var stats);
            
            if (available.HasValue)
                query.WhereLucene("Option_Available", available.ToString());

            foreach (var filter in filters)
            {
                query.WhereLucene(filter.Key, $"({string.Join(" OR ", filter.Value)})");
            }

            return query.AggregateBy(GetFacets()).Execute();
        }

        private IEnumerable<Facet> GetFacets()
        {
            var facets = new List<Facet>();

            facets.Add(new Facet
            {
                FieldName = "Brand",
                DisplayFieldName = "Brand"
            });
            facets.Add(new Facet
            {
                FieldName = "Option_Available",
                DisplayFieldName = "Availability"
            });
            facets.Add(new Facet
            {
                FieldName = "Size",
                DisplayFieldName = "Size"
            });
            facets.Add(new Facet
            {
                FieldName = "Color",
                DisplayFieldName = "Color"
            });

            return facets;
        }
    }
}
