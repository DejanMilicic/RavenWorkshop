using System.Collections.Generic;
using Raven.Client.Documents;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace Northwind.Features.RavenDocumentStore
{
    public static class RavenDocumentStore
    {
        // You can run RavenDB completely in memory
        // by setting key in settings.json file
        //  {
        //      "ServerUrl": "http://localhost:8080",
        //      "RunInMemory": true
        //  }
        // when you start RavenDB with this configuration
        // all databases that are created in memory
        // and none of them are stored on the disk.
        // If you restart server, everything will be lost
        // https://ravendb.net/docs/article-page/latest/Csharp/server/configuration/core-configuration#runinmemory

        public static IDocumentStore GetStore()
        {
            // In-memory instance of RavenDB is initialized in a regular way
            // Settings and parameters will be picked up from settings.json
            return new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" }
            }.Initialize();
        }

        // It is also possible to run RavenDB server in a standard mode
        // and have just some of the databases in memory.
        // Here you can see how to create new database that will run in memory.
        // This database will store all documents in the memory, and after you restart server
        // you will find that this database exists, but it is completely empty
        // In memory databases could be used for running tests.
        // Integration tests with in-memory tests are supported via https://www.nuget.org/packages/RavenDB.TestDriver/
        public static void CreateInMemoryDatabase()
        {
            GetStore().Maintenance.Server.Send(
                new CreateDatabaseOperation(
                    new DatabaseRecord("MyTestDatabase")
                    { Settings = new Dictionary<string, string> { { "RunInMemory", "true" } } }
                    ));
        }
    }
}
