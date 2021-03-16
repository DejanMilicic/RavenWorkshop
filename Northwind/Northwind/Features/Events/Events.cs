using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System;
using System.Linq;

namespace Northwind.Features.Events
{
    public class Events
    {
        public void Create()
        {
            using var session = Dsh.Store.OpenSession();

            Employee emp = new Employee();
            emp.FirstName = "Marco";
            emp.LastName = "Polo";

            session.Store(emp);
            session.SaveChanges();
        }

        public void Delete()
        {
            using var session = Dsh.Store.OpenSession();

            session.Delete("employees/6-A");
            session.SaveChanges();
        }

        public void Query()
        {
            using var session = Dsh.Store.OpenSession();

            var employees = session.Query<Employee>().ToList();

            Console.WriteLine(employees.Count);
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

                //store.OnBeforeConversionToDocument += (sender, eventArgs) =>
                //{
                //    if (eventArgs.Entity is Employee employee)
                //        employee.FirstName = employee.FirstName + "_2";
                //};

                //store.OnBeforeStore += (sender, eventArgs) =>
                //{
                //    if (eventArgs.Entity is Employee employee)
                //        if (employee.HiredAt == default)
                //            employee.HiredAt = DateTime.UtcNow;
                //};

                // Hands On 1 : Add the client ID of the user before storing the user
                store.OnBeforeStore += (sender, e) =>
                {
                    if (e.Session.GetChangeVectorFor(e.Entity) == null)
                        e.DocumentMetadata["Created-By"] = "currentUser";
                    e.DocumentMetadata["Modified-By"] = "currentUser";
                };

                // Hands On 2 : Prevent deletion of Employees with a specific name
                store.OnBeforeDelete += (sender, e) =>
                {
                    if (e.Entity is Employee)
                        throw new InvalidOperationException();
                };

                store.OnBeforeQuery += (sender, e) =>
                {
                    if (e.QueryCustomization is IDocumentQuery<Employee> qe)
                    {
                        qe.AndAlso().WhereEquals("Address.City", "London");
                    }
                };

                store.Initialize();

                IndexCreation.CreateIndexes(typeof(Program).Assembly, store);

                return store;
            });

        public static IDocumentStore Store => LazyStore.Value;
    }
}
