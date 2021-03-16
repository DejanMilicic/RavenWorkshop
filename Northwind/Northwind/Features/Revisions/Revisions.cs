using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;

namespace Northwind.Features.Revisions
{
    public class Revisions
    {
        public void ForceCreation()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee employee = new Employee
            {
                FirstName = "Rev",
                LastName = "Revision"
            };

            session.Store(employee);
            session.SaveChanges();

            session.Advanced.Revisions.ForceRevisionCreationFor(employee);
            session.SaveChanges();

            employee.FirstName = "Rev2";
            session.SaveChanges();
        }

        public void GetRevisions()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var revisions = session.Advanced.Revisions.GetFor<Employee>("employees/8-A");

            foreach (Employee revision in revisions)
            {
                Console.WriteLine(revision);
            }
        }
    }
}
