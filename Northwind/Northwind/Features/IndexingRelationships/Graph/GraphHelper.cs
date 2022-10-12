using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Documents.Queries;
using Northwind.Features.IndexingRelationships.Graph;
using Raven.Server.Documents.Indexes.Static;

namespace Northwind.Features.IndexingRelationships.Graph
{
    public class Entry
    {
        public string Ancestor { get; set; }

        public string Distance { get; set; }

        public string Descendant { get; set; }
    }

    public static class GraphHelper
    {
        public static List<Entry> Process(Northwind.Features.IndexingRelationships.Graph.Number number)
        {
            var res = new List<Entry>();

            CurrentIndexingScope scope = CurrentIndexingScope.Current;

            dynamic doc = scope.LoadDocument(null, number.Id, "Numbers");

            string[] followedBy = doc.FollowedBy;

            if (followedBy.Any())
            {
                foreach (string num in followedBy)
                {
                    res.Add(
                        new Entry
                        {
                            Ancestor = number.Id,
                            Distance = "1",
                            Descendant = num
                        }
                    );
                }
            }

            return res;
        }
    }
}
