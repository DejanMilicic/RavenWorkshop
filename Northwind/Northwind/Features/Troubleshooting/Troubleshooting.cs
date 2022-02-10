using System;
using Northwind.Models.Entity;

namespace Northwind.Features.Troubleshooting
{
    public static class Troubleshooting
    {
        public static void RequestSize()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee emp = session.Load<Employee>("employees/8-A");
            emp.FirstName = Guid.NewGuid().ToString();

            session.Advanced.DocumentStore.GetRequestExecutor().OnBeforeRequest += (s, e) =>
            {
                Console.WriteLine($"Request size: {e.Request.Content?.ReadAsStringAsync().Result.Length ?? 0}b");
            };

            session.SaveChanges();
        }
    }
}
