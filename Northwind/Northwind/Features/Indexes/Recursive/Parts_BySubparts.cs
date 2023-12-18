using Raven.Client.Documents.Indexes;
using System.Collections.Generic;
using System.Linq;

namespace Northwind.Features.Indexes.Recursive;

public class Parts_BySubparts : AbstractIndexCreationTask<Part, Parts_BySubparts.Entry>
{
    public class Entry
    {
        public string Id { get; set; }

        public List<string> Subparts { get; set; }
    }

    public Parts_BySubparts()
    {
        Map = parts =>
            from part in parts
                let subparts = Recurse(
                    part, part => part.SubParts.Select(subpartId => LoadDocument<Part>(subpartId)))
                let subpartIds = subparts.Select(x => x.SubParts)

            select new Entry
            {
                Id = part.Id,
                Subparts = subpartIds.SelectMany(x => x).ToList()
            };

        Store(x => x.Id, FieldStorage.Yes);
        Store(x => x.Subparts, FieldStorage.Yes);
    }
}
