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
        public string Ancestor { get; set; }

        public string Distance { get; set; }

        public string Descendant { get; set; }
    }

    public static class GraphHelper
    {
        public static List<Entry> Process(Northwind.Features.IndexingRelationships.Graph.Number number)
        {
            var res = new List<Entry>();


            var scope = Raven.Server.Documents.Indexes.Static.CurrentIndexingScope.Current;
            //dynamic document = (Raven.Server.Documents.Indexes.Static.DynamicArray)scope.LoadDocument(null, "1", "Numbers");
            //var document = scope.LoadDocument(null, number.Id, "Numbers");
            //var cd = (IEnumerable<KeyValuePair<object, object>>)document;
            //List<KeyValuePair<object, object>> list = cd.ToList();
            //var el = list.Skip(1).Take(1).Single();

            //var xx = list.SingleOrDefault(x => x.Key == "folowedby");

            dynamic doc = scope.LoadDocument(null, number.Id, "Numbers");

            string[] followedBy = doc.FollowedBy;

            if (followedBy.Any())
            {
                foreach (string num in followedBy)
                {
                    res.Add(
                        new Entry
                        {
                            Ancestor = number.Id,
                            Distance = "1",
                            Descendant = num
                        }
                    );
                }
            }

            return res;


            //string yy = xx.Value.ToString() ?? ":(";


            //var array = ((DynamicArray)el.Value);
            //if (array.Any())
            //{
            //    //var a = ((DynamicArray)el.Value)[0];

            //    foreach (object num in ((DynamicArray)el.Value))
            //    {
            //        res.Add(
            //            new Entry
            //            {
            //                Ancestor = number.Id,
            //                //Distance = (((IEnumerable<object>)xx.Value).ToList())[0].ToString(),
            //                Distance = "yy",
            //                Descendant = num.ToString()
            //            }
            //            );
            //    }
            //}

            return res;
        }
    }
}
