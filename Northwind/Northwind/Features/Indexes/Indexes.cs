using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Operations.Indexes;

namespace Northwind.Features.Indexes
{
    public class Indexes
    {
        public void GetStaleIndexes()
        {
            var stats = DocumentStoreHolder.Store.Maintenance.Send(new GetStatisticsOperation());

            string[] staleIndexes = stats.StaleIndexes;
        }

        public static void ResetIndex(string indexName)
        {
            DocumentStoreHolder.Store.Maintenance.Send(new ResetIndexOperation(indexName));
        }
    }
}
