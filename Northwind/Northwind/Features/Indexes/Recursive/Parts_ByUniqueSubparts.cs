using System;
using Raven.Client.Documents.Indexes;
using System.Collections.Generic;
using System.Linq;

namespace Northwind.Features.Indexes.Recursive;

public class Parts_ByUniqueSubparts : AbstractIndexCreationTask<Part, Parts_ByUniqueSubparts.Entry>
{
    public class Subpart
    {
        public string ParentId { get; set; }

        public List<string> SubpartIds { get; set; }
    }

    public class Entry
    {
        public string Id { get; set; }

        public List<Subpart> Subparts { get; set; }
    }

    public Parts_ByUniqueSubparts()
    {
        Map = parts =>
            from part in parts
                let subparts = Recurse(
                    part, part => part.SubParts.Select(subpartId => LoadDocument<Part>(subpartId)))
                let subpartIds = subparts.Select(x => x.SubParts)

            select new Entry
            {
                Id = part.Id,
                Subparts = subpartIds.Select(x => new Subpart
                {
                    ParentId = part.Id,
                    SubpartIds = x.ToList()
                }).ToList()
            };

        Store(x => x.Id, FieldStorage.Yes);
        Store(x => x.Subparts, FieldStorage.Yes);
    }
}
