using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using System;
using Northwind.Models.Entity;

namespace Northwind.Features.Events
{
    public class Events
    {
        public void Demo1()
        {
            using var session = Dsh.Store.OpenSession();

            Employee emp = new Employee();
            emp.FirstName = "Marco";
            emp.LastName = "Polo";

            session.Store(emp);
            session.SaveChanges();
        }
    }

    public static class Dsh
    {
        private static readonly Lazy<IDocumentStore> LazyStore =
            new Lazy<IDocumentStore>(() =>
            {
                IDocumentStore store = new DocumentStore
                {
                    Urls = new[] { "http://127.0.0.1:8080" },
                    Database = "demo"
                };

                store.OnBeforeConversionToDocument += (sender, eventArgs) =>
                {
                    if (eventArgs.Entity is Employee employee)
                        employee.FirstName = employee.FirstName + "_2";
                };

                store.OnBeforeStore += (sender, eventArgs) =>
                {
                    if (eventArgs.Entity is Employee employee)
                        if (employee.HiredAt == default)
                            employee.HiredAt = DateTime.UtcNow;
                };

                store.Initialize();

                IndexCreation.CreateIndexes(typeof(Program).Assembly, store);

                return store;
            });

        public static IDocumentStore Store => LazyStore.Value;
    }
}
