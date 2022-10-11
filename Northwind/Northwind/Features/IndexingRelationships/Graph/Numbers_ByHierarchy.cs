using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.IndexingRelationships.Graph
{
    public class Numbers_ByHierarchy : AbstractJavaScriptIndexCreationTask
    {
        public class Result
        {
            public IEnumerable<string> Followers { get; set; }
        }

        public Numbers_ByHierarchy()
        {
            Maps = new HashSet<string>
            {
                @"
map('Numbers', function (num) {
    var numbers = [];

    var follower = load(num.IsFollowedBy, 'Numbers');

    while (follower != null) {
        numbers.push(id(follower));
        follower = load(follower.IsFollowedBy, 'Numbers');
    }

    return {
        Followers: numbers
    };
});
"
            };
        }
    }

}
