using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Northwind.Features
{
    public partial class GenericPaging<T>
    {
        public Task<List<T>> GetALl(int pageIndex, int pageSize, out string queryStats)
        {
            var store = new DocumentStore
            {
                Urls = new string[] { "https://a.free.dejanmilicic.ravendb.cloud/" },
                Certificate = new X509Certificate2("free.dejanmilicic.client.certificate.pfx"),
                Database = "demo"
            }.Initialize();

            using (IAsyncDocumentSession session = store.OpenAsyncSession())
            {
                var results = session
                    .Query<T>()
                    .Statistics(out QueryStatistics stats)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                queryStats = stats.TotalResults.ToString();
                return results;
            }
        }
    }
}
