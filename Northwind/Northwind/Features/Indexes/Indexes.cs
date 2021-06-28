using Raven.Client.Documents.Operations;

namespace Northwind.Features.Indexes
{
    public class Indexes
    {
        public void GetStaleIndexes()
        {
            var stats = DocumentStoreHolder.Store.Maintenance.Send(new GetStatisticsOperation());

            string[] staleIndexes = stats.StaleIndexes;
        }
    }
}
