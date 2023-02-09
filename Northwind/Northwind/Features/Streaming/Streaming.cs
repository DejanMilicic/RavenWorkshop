using Raven.Client.Documents.Commands;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using Northwind.Models.Entity;

namespace Northwind.Features.Streaming
{
    /// <summary>
    /// https://ravendb.net/docs/article-page/5.4/Csharp/client-api/session/querying/how-to-stream-query-results
    /// </summary>
    public static class Streaming
    {
        public static void Do()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            // Define a query on a collection
            IRavenQueryable<Employee> query = session
            .Query<Employee>()
            .Where(x => x.FirstName == "Robert");

            // Call 'Stream' to execute the query
            // Optionally, pass an 'out param' for getting the query stats
            IEnumerator<StreamResult<Employee>> streamResults =
                session.Advanced.Stream(query, out StreamQueryStatistics streamQueryStats);

            // Read from the stream
            while (streamResults.MoveNext())
            {
                // Process the received result
                StreamResult<Employee> currentResult = streamResults.Current;

                // Get the document from the result
                // This entity will Not be tracked by the session
                Employee employee = currentResult.Document;

                // The currentResult item also provides the following:
                var employeeId = currentResult.Id;
                var documentMetadata = currentResult.Metadata;
                var documentChangeVector = currentResult.ChangeVector;

                // Can get info from the stats, i.e. get number of total results
                int totalResults = streamQueryStats.TotalResults;
                // Get the Auto-Index that was used/created with this dynamic query
                string indexUsed = streamQueryStats.IndexName;
            }
        }
    }
}
