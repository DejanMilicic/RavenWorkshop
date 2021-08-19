using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Search
{
    public class Omnisearch : AbstractMultiMapIndexCreationTask<Omnisearch.Entry>
    {
        public class Projection
        {
            public string Id { get; set; }

            public string DisplayName { get; set; }

            public string Collection { get; set; }
        }

        public class Entry
        {
            public string Id { get; set; }

            public string DisplayName { get; set; }

            public object Collection { get; set; }

            public string[] Content { get; set; }
        }

        public Omnisearch()
        {
            AddMap<Company>(companies => from c in companies
                select new Entry
                {
                    Id = c.Id,
                    Content = new[] { c.Name },
                    DisplayName = c.Name,
                    Collection = MetadataFor(c)["@collection"]
                });

            AddMap<Product>(products => from p in products
                select new Entry
                {
                    Id = p.Id,
                    Content = new[] { p.Name },
                    DisplayName = p.Name,
                    Collection = MetadataFor(p)["@collection"]
                });

            AddMap<Employee>(employees => from e in employees
                select new Entry
                {
                    Id = e.Id,
                    Content = new[]
                    {
                        e.FirstName,
                        e.LastName
                    },
                    DisplayName = e.FirstName + " " + e.LastName,
                    Collection = MetadataFor(e)["@collection"]
                });

            Index(x => x.Content, FieldIndexing.Search);

            // storing fields so when projection (e.g. ProjectInto)
            // requests only those fields
            // then data will come from index only, not from storage
            Store(x => x.Id, FieldStorage.Yes);
            Store(x => x.DisplayName, FieldStorage.Yes);
            Store(x => x.Collection, FieldStorage.Yes);
        }
    }
}
