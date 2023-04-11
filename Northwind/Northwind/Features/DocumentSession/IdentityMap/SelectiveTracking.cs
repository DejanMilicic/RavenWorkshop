using Northwind.Models.Entity;
using Raven.Client.Documents;

namespace Northwind.Features.DocumentSession.IdentityMap;

public static class SelectiveTracking
{
    public static void Demo()
    {
        using var store = new DocumentStore { Urls = new[] { "http://127.0.0.1:8080" }, Database = "demo" };
        store.Conventions.ShouldIgnoreEntityChanges = (session, entity, id) => (entity is Employee { FirstName: "Laura" });
        store.Initialize();

        using var session = store.OpenSession();

        Employee laura = session.Load<Employee>("employees/8-A"); // laura will not be tracked for changes
        laura.LastName += " CHANGED";

        Employee robert = session.Load<Employee>("employees/7-A");
        robert.LastName += " CHANGED";

        Supplier supplier = session.Load<Supplier>("suppliers/1-A");
        supplier.Name += " CHANGED";

        session.SaveChanges();
    }
}

