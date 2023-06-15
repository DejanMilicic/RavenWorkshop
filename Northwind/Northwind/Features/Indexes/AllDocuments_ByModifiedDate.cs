using System;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace Northwind.Features.Indexes;

public class AllDocuments_ByModifiedDate : AbstractIndexCreationTask<object, AllDocuments_ByModifiedDate.Entry>
{
    public class Entry
    {
        public DateTime ModifiedOn { get; set; }
    }

    public AllDocuments_ByModifiedDate()
    {
        Map = docs => 
            from doc in docs 
            select new Entry
            {
                ModifiedOn = MetadataFor(doc).Value<DateTime>("@last-modified")
            };
    }
}

