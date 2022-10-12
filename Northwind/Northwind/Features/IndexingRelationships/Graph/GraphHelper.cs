using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Features.IndexingRelationships.Graph;

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
        public static Entry Process()
        {
            return new Entry
            {
                Ancestor = "1x",
                Distance = "2x",
                Descendant = "3x"
            };
        }
    }
}
