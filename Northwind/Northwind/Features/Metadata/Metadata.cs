using System;
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
            
            // todo: add example with json structure being stored to metadata

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
