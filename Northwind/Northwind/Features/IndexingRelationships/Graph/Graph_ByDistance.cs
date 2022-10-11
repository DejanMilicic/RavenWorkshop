using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.IndexingRelationships.Graph
{
    public class Graph_ByDistance : AbstractJavaScriptIndexCreationTask
    {
        public class Result
        {
            public string Ancestor { get; set; }

            public string Distance { get; set; }

            public string Descendant { get; set; }
        }

        public Graph_ByDistance()
        {
            Maps = new HashSet<string>
            {
                @"
map('Numbers', function (num) {
    var graph = [];

    var ancestor = id(num);
    var descendant = load(num.IsFollowedBy, 'Numbers');
    var distance = 1;

    while (descendant != null) {
        graph.push({
            Ancestor: ancestor,
            Distance: distance,
            Descendant: id(descendant)
        });        

        distance++;
    
        descendant = load(descendant.IsFollowedBy, 'Numbers');
    }

    return {
        Graph: graph
    };
});
"
            };
        }
    }

}
