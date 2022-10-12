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
        public Entry(string ancestor, int distance, string descendant)
        {
            this.Ancestor = ancestor;
            this.Distance = distance.ToString();
            this.Descendant = descendant;
        }

        public string Ancestor { get; set; }

        public string Distance { get; set; }

        public string Descendant { get; set; }
    }

    public static class GraphHelper
    {
        public static List<Entry> Process(Northwind.Features.IndexingRelationships.Graph.Number number)
        {
            List<Entry> res = new List<Entry>();
            Queue<(int distance, string doc)> queue = new();

            CurrentIndexingScope scope = CurrentIndexingScope.Current;
            
            queue.Enqueue((0, number.Id));

            while (queue.Any())
            {
                var current = queue.Dequeue();

                dynamic doc = scope.LoadDocument(null, current.doc, "Numbers");

                string[] followedBy = doc.FollowedBy;

                if (followedBy.Any())
                {
                    foreach (string descendant in followedBy)
                    {
                        res.Add(new Entry(number.Id, current.distance + 1, descendant));
                        queue.Enqueue((current.distance + 1, descendant));
                    }
                }
            }

            return res;
        }
    }
}
