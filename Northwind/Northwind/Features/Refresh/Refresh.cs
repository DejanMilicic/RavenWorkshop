using System;
using Northwind.Models.Entity;

namespace Northwind.Features.Refresh
{
    public class Refresh
    {
        public void SetRefresh()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var employee = session.Load<Employee>("employees/8-A");
            session.Advanced.GetMetadataFor(employee)["@refresh"] = DateTime.UtcNow.AddMinutes(1);
            session.SaveChanges();
        }
    }
}
