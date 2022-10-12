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
                let zzz = GraphHelper.Process()
                select new
                {
                    Ancestor = zzz.Ancestor,
                    Distance = zzz.Distance,
                    Descendant = zzz.Descendant
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
