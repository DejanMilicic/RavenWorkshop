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
    public class Graph_ByDistance2 : AbstractIndexCreationTask<Flight>
    {
        public Graph_ByDistance2()
        {
            Map = numbers => from number in numbers
                             let entries = GraphHelper.Process(number)
                             from entry in entries
                             select new
                             {
                                 Origin = entry.Origin,
                                 Distance = entry.Distance,
                                 Destination = entry.Destination
                             };

            AdditionalSources = new Dictionary<string, string>
            {
                ["GraphHelper.cs"] =
                    File.ReadAllText(Path.Combine(new[]
                        { AppContext.BaseDirectory, "..", "..", "..", "Features", "IndexingRelationships", "Graph", "GraphHelper.cs" }))
            };

            //AdditionalAssemblies = new HashSet<AdditionalAssembly>
            //{
            //    AdditionalAssembly.FromPath(typeof(Flight).Assembly...GetName().ToString())
            //};
        }
    }
}
