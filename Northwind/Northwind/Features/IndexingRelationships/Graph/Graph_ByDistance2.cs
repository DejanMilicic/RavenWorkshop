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
    public class Graph_ByDistance2 : AbstractIndexCreationTask<Number, Graph_ByDistance2.Entry>
    {
        public class Entry
        {
            public string Ancestor { get; set; }

            public string Distance { get; set; }

            public string Descendant { get; set; }
        }

        public Graph_ByDistance2()
        {
            Map = numbers => from number in numbers
                select new Entry
                {
                    Ancestor = "1",
                    Distance = "2",
                    Descendant = "3"
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
