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
        public static List<Entry> Process()
        {
            return new List<Entry>
            {
                new Entry
                {
                    Ancestor = "x",
                    Distance = "x",
                    Descendant = "x"
                },
                new Entry
                {
                    Ancestor = "y",
                    Distance = "y",
                    Descendant = "y"
                }
            };
        }
    }
}
