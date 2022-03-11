using System.Collections.Generic;
using Raven.Client.Documents;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace Northwind.Features.RavenDocumentStore
{
    public static class RavenDocumentStore
    {
        public static IDocumentStore GetStore()
        {
            return new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" }
            }.Initialize();
        }

        public static void CreateInMemoryDatabase()
        {
            GetStore().Maintenance.Server.Send(
                new CreateDatabaseOperation(
                    new DatabaseRecord("MyTestDatabase") 
                        { Settings = new Dictionary<string, string>{ { "RunInMemory", "true" } }}
                    ));
        }
    }
}
