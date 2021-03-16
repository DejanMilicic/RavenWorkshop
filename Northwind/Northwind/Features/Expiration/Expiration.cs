using System;
using Northwind.Models.Entity;

namespace Northwind.Features.Expiration
{
    public class Expiration
    {
        public void SetExpiration()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var employee = session.Load<Employee>("employees/42-A");
            session.Advanced.GetMetadataFor(employee)["@expires"] = DateTime.UtcNow.AddMinutes(1);
            session.SaveChanges();
        }
    }
}
