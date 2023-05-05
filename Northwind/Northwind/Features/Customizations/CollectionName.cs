using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;

namespace Northwind.Features.Customizations;

public static class CollectionName
{
    public static void Convention()
    {
        IDocumentStore store = new DocumentStore
        {
            Urls = new[] { "http://127.0.0.1:8080" },
            Database = "demo",
            Conventions = 
            {
                FindCollectionName = type =>
                {
                    if (type == typeof(Employee))
                        return "Workers";
                    return DocumentConventions.DefaultGetCollectionName(type);
                }
            }
        };

        store.Initialize();

        Employee john = new Employee();
        john.FirstName = "John";
        john.LastName = "Doe";

        using var session = store.OpenSession();
        session.Store(john);
        session.SaveChanges();
    }

    public static void Individual()
    {

    }
}

