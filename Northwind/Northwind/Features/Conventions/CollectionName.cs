using System.Diagnostics;
using Northwind.Models.Entity;
using Raven.Client.Documents;

namespace Northwind.Features.Conventions;

internal static class CollectionNameConvention
{
    private static IDocumentStore GetStore()
    {
        IDocumentStore store = new DocumentStore
        {
            Urls = new[] { "http://127.0.0.1:8080" },
            Database = "demo",
            Conventions = {
                FindCollectionName = type => type.Name
            }
        };

        store.Initialize();

        return store;
    }

    public static void Demo()
    {
        Employee emp = new Employee
        {
            FirstName = "Marco",
            LastName = "Polo"
        };

        using var session = GetStore().OpenSession();

        session.Store(emp);
        session.SaveChanges();
    }
}

