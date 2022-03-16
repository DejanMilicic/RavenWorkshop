using Raven.Client.Documents;
using Raven.Client.Documents.Operations.ConnectionStrings;
using Raven.Client.Documents.Operations.ETL;

namespace Northwind.Features.EtlRaven
{
    // ETL Basics: https://ravendb.net/docs/article-page/4.2/csharp/server/ongoing-tasks/etl/basics
    // RavenDB ETL: https://ravendb.net/docs/article-page/4.2/csharp/server/ongoing-tasks/etl/raven
    public static class EtlRaven
    {
        public static void EmployeesAndOrders()
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
                        Name = "loadAllEmployees",
                        Collections = {"Employees"}
                    },
                    {
                        new Transformation()
                        {
                            Name = "TransformOrders",
                            Collections = {"Orders"},
                            Script = @"
var c = load(this.Company);
this.CompanyName = c.Name;
delete(this.Company);
loadToOrders(this);
"
                        }
                    }
                }
            };

            source.Maintenance.Send(new AddEtlOperation<RavenConnectionString>(etlConfiguration));
        }
    }
}
