using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Features.Recursive;
using Northwind.Models.Entity;

namespace Northwind.Features.Metadata
{
    public class Metadata
    {
        public void Create()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();
            var emp = session.Load<Employee>("employees/8-A");

            session.Advanced.GetMetadataFor(emp)["IsDeleted"] = "true";

            session.SaveChanges();
        }

        public void Read()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();
            var emp = session.Load<Employee>("employees/8-A");

            string isDeleted = (string)session.Advanced.GetMetadataFor(emp)["IsDeleted"];

            Console.WriteLine($"{emp.FirstName}: {isDeleted}");
        }
    }
}
