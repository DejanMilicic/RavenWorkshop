using Raven.Client.Documents.Indexes;
using System.Collections.Generic;

namespace Northwind.Features.Indexes.DynamicFields.HeterogeneousDocuments
{
    public class Documents_ByAnyField : AbstractJavaScriptIndexCreationTask
    {
        public Documents_ByAnyField()
        {
            // This will index EVERY FIELD under the top level of the document
            Maps = new HashSet<string>
            {
                @"
                map('@all_docs', function (d) {
                    return {
                        _: Object.keys(d).map(key => createField(key, d[key],
                          { indexing: 'Search', storage: true, termVector: null }))
                    }
                })"
            };
        }
    }
}
