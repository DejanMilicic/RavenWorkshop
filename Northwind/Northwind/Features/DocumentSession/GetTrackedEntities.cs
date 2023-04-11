using System.Linq;
using Raven.Client.Documents;

namespace Northwind.Features.DocumentSession;

public static class GetTrackedEntities
{
    public static void Demo()
    {
        using var store = new DocumentStore { Urls = new[] { "http://127.0.0.1:8080" }, Database = "demo" }.Initialize();

        using var session = store.OpenSession();

        Patient firstPatient = new Patient { Name = "John", Description = "original desc" };
        session.Store(firstPatient);

        // for second patient, add it only if it already does not exist

        Patient secondPatient = new Patient { Name = "John", Description = "edited desc" };

        var exists = session.Advanced
            .GetTrackedEntities()
            .Select(x => x.Value)
            .Any(x => x.Entity is Patient patient && patient.Name == secondPatient.Name);

        if (!exists)
            session.Store(secondPatient);

        session.SaveChanges();
    }
}

public class Patient
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}