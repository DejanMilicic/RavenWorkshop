using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Linq;
using System.Linq;
using Northwind.Models.Entity;

namespace Northwind.Features.Indexes.IndexingMetadata;

public static class IndexingMetadataDemo
{
    private static IDocumentStore GetStore()
    {
        var store = (new DocumentStore
        {
            Urls = new[] { "http://127.0.0.1:8080" },
            Database = "demo"
        }).Initialize();

        IndexCreation.CreateIndexes(new AbstractIndexCreationTask[] { new Employees_BySqlKeys() }, store);
        
        return store;
    }

    public static void Demo()
    {
        /*
            Add following to the @metadata of "employees/8-a"

                "@sql-keys": {
                    "id": 251
                } 
         */

        using var session = GetStore().OpenSession();

        var employeesForUpdate = session.Query<Employees_BySqlKeys.Entry, Employees_BySqlKeys>()
            .Where(x => x.SqlKeyId == "251")
            .ProjectInto<Employee>()
            .ToList();
    }
}

