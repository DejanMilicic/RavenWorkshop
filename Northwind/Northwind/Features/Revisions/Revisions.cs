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

        public void UndoRedoSim()
        {
            string employeeId;
            Employee employee;

            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                // create new employee
                employee = new Employee();
                employee.FirstName = "First";
                session.Store(employee);
                employeeId = employee.Id;
                session.SaveChanges();

                // turn revisions on
                session.Advanced.Revisions.ForceRevisionCreationFor(employee);
                session.SaveChanges();
            }

            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                employee = session.Load<Employee>(employeeId);
                employee.FirstName = "Second";
                session.SaveChanges();
            }

            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                employee = session.Load<Employee>(employeeId);
                employee.FirstName = "Third";
                session.SaveChanges();
            }

            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var revisions = session.Advanced.Revisions.GetFor<Employee>(employeeId);
                var firstEmployeeRevision = revisions.Skip(1).First();
                session.Store(firstEmployeeRevision);
                session.SaveChanges();
            }
        }

        public void GetRevisionsForEmployee()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var revisions = session.Advanced.Revisions.GetFor<Employee>("employees/8-A");

            foreach (Employee revision in revisions)
            {
                Console.WriteLine(revision);
            }
        }

        public void GetRevisionsForOrder()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            List<Order> orderRevisions = session.Advanced.Revisions.GetFor<Order>("orders/823-A");

            foreach (Order revision in orderRevisions)
            {
                Console.WriteLine(revision.Lines.Count);
            }
        }
    }
}
