using System;
using Northwind.Models.Entity;

namespace Northwind.Features.Expiration
{
    // todo : add example of log entries 
    // different life based on the logging level

    public class Expiration
    {
        public void SetExpiration()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var employee = session.Load<Employee>("employees/8-A");
            session.Advanced.GetMetadataFor(employee)["@expires"] = DateTime.UtcNow.AddSeconds(20);
            session.SaveChanges();
        }
    }
}
