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
        public Entry(string origin, int distance, string destination)
        {
            this.Origin = origin;
            this.Distance = distance.ToString();
            this.Destination = destination;
        }

        public string Origin { get; set; }

        public string Distance { get; set; }

        public string Destination { get; set; }
    }

    public static class GraphHelper
    {
        public static List<Entry> Process(Northwind.Features.IndexingRelationships.Graph.Flight flight)
        {
            List<Entry> res = new List<Entry>();
            Queue<(int distance, string doc)> queue = new();

            CurrentIndexingScope scope = CurrentIndexingScope.Current;
            
            queue.Enqueue((0, flight.Id));

            while (queue.Any())
            {
                var current = queue.Dequeue();

                dynamic doc = scope.LoadDocument(null, current.doc, "Flights");

                string[] to = doc.To;

                if (to.Any())
                {
                    foreach (string destination in to)
                    {
                        res.Add(new Entry(flight.Id, current.distance + 1, destination));
                        queue.Enqueue((current.distance + 1, destination));
                    }
                }
            }

            return res;
        }
    }
}
