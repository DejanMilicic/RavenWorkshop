using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;

namespace Northwind.Features.Identifiers
{
    public class Identifiers
    {
        public void NullId()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee emp = new Employee();
            emp.Id = null;
            emp.FirstName = "Jane";
            emp.LastName = "Doe";

            session.Store(emp);
            session.SaveChanges();
        }

        public void EmptyStringId()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee emp = new Employee();
            emp.Id = "";
            emp.FirstName = "Jane";
            emp.LastName = "Doe";

            session.Store(emp);
            session.SaveChanges();
        }

        public void ValueId()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee emp = new Employee();
            emp.Id = "230600A_56";
            emp.FirstName = "Jane";
            emp.LastName = "Doe";

            session.Store(emp);
            session.SaveChanges();
        }

        public void PrefixId()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee emp = new Employee();
            emp.Id = "Employees/Usa/California/Office1/";
            emp.FirstName = "Jane";
            emp.LastName = "Doe";

            session.Store(emp);
            session.SaveChanges();
        }

        public void SlashId()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee emp = new Employee();
            emp.Id = "/";
            emp.FirstName = "Jane";
            emp.LastName = "Doe";

            session.Store(emp);
            session.SaveChanges();
        }

        public void VerticalBarId()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee emp = new Employee();
            emp.Id = "|";
            emp.FirstName = "Jane";
            emp.LastName = "Doe";

            session.Store(emp);
            session.SaveChanges();
        }

        public void CollectionVerticalBarId()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee emp = new Employee();
            emp.Id = "Employees|";
            emp.FirstName = "Jane";
            emp.LastName = "Doe";

            session.Store(emp);
            session.SaveChanges();
        }
    }
}
