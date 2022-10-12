using Northwind.Features.AdditionalSources;
using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.IndexingRelationships.Graph
{
    public class Graph_ByDistance2 : AbstractIndexCreationTask<Number>
    {
        public Graph_ByDistance2()
        {
            Map = numbers => from number in numbers
                             let entries = GraphHelper.Process()
                             from entry in entries
                             select new
                             {
                                 Ancestor = entry.Ancestor,
                                 Distance = entry.Distance,
                                 Descendant = entry.Descendant
                             };

            AdditionalSources = new Dictionary<string, string>
            {
                ["GraphHelper.cs"] =
                    File.ReadAllText(Path.Combine(new[]
                        { AppContext.BaseDirectory, "..", "..", "..", "Features", "IndexingRelationships", "Graph", "GraphHelper.cs" }))
            };
        }
    }
}
