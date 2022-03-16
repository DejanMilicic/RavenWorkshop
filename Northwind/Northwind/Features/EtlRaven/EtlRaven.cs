using Raven.Client.Documents;
using Raven.Client.Documents.Operations.ConnectionStrings;
using Raven.Client.Documents.Operations.ETL;

namespace Northwind.Features.EtlRaven
{
    // ETL Basics: https://ravendb.net/docs/article-page/4.2/csharp/server/ongoing-tasks/etl/basics
    // RavenDB ETL: https://ravendb.net/docs/article-page/4.2/csharp/server/ongoing-tasks/etl/raven
    public static class EtlRaven
    {
        public static void EmployeesWithoutTransformationScript()
        {
            IDocumentStore source = new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "source"
            }.Initialize();

            var connectionString = new RavenConnectionString
            {
                Name = "destination",
                Database = "destination",
                TopologyDiscoveryUrls = new[] { "http://127.0.0.1:8080" }
            };

            source.Maintenance.Send(new PutConnectionStringOperation<RavenConnectionString>(connectionString));

            var etlConfiguration = new RavenEtlConfiguration
            {
                ConnectionStringName = connectionString.Name,
                Transforms =
                {
                    new Transformation()
                    {
                        Name = "loadAll",
                        Collections = {"Employees"}
                    }
                }
            };

            source.Maintenance.Send(new AddEtlOperation<RavenConnectionString>(etlConfiguration));
        }
    }
}
