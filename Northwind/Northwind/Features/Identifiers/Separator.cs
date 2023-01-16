using Northwind.Models.Entity;
using Raven.Client.Documents;

namespace Northwind.Features.Identifiers
{
    public static class Separator
    {
        private static IDocumentStore GetStore()
        {
            var store = (new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "Demo"
            });

            store.Conventions.IdentityPartsSeparator = '-';

            store.Initialize();

            return store;
        }

        public static void Demo()
        {
            Employee emp = new Employee();
            emp.FirstName = "Seb";
            emp.LastName = "Separator";

            using var session = GetStore().OpenSession();

            session.Store(emp);
            session.SaveChanges();
        }
    }
}
