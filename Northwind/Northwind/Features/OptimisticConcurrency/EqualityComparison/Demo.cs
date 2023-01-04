using System;
using System.Linq;
using Raven.Client.Documents;

namespace Northwind.Features.OptimisticConcurrency.EqualityComparison
{
    public static class EqualityComparison
    {
        public static void Demo()
        {
            var store = new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "Demo"
            }.Initialize();

            Patient pa = new Patient
            {
                Id = Guid.NewGuid().ToString(),
                Name = "John",
                Description = "original desc"
            };

            Patient pb = new Patient
            {
                Id = Guid.NewGuid().ToString(),
                Name = "John",
                Description = "edited desc"
            };

            using var session = store.OpenSession();
            session.Store(pa);

            var exists = session.Advanced
                .GetTrackedEntities()
                .Select(x => x.Value)
                .Any(x => x.Entity is Patient patient && patient.Name == pb.Name);

            if (!exists)
                session.Store(pb);

            session.SaveChanges();
        }
    }
}
