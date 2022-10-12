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
    //var descendants = load(num.To.join(','), 'Numbers');
    //var descendants = num.To.forEach(x => load(x, 'Numbers'));

    var obj = load('1', 'Numbers');
    var descendants = obj.To.forEach(x => id(load(x, 'Numbers')));
    var distance = 1;

        graph.push({
            Origin: ancestor,
            Distance: distance,
            Descendants: descendants
        });  

    return graph;

    while (descendants.length > 0) {
        //graph.push({
        //    Origin: ancestor,
        //    Distance: distance,
        //    //Descendants: id(descendant)
        //    Descendants: descendants
        //});        

        distance++;
    
        descendant = load(descendant.To.join(','), 'Numbers');
    }

    return graph;
});
"
            };
        }
    }

}
