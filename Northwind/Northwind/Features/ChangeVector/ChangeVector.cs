using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;

namespace Northwind.Features.ChangeVector
{
    // The change-vector reflects the cluster wide point in time where something happened.
    // It includes the unique database ID, node identifier, and the Etag of the document
    // in the specific node. When a document is downloaded from the server, it contains
    // various metadata information e.g. ID or current change-vector.
    //
    // Current change-vector is stored within the metadata in session and is available
    // for each entity using the GetChangeVectorFor method from the Advanced session operations.
    public class ChangeVector
    {
        public void Do()
        {
            var store = DocumentStoreHolder.GetStore();
            store.Initialize();

            using (var session = store.OpenSession())
            {
                Employee emp = new Employee();
                // newly created entity, change vector does not exist

                session.Store(emp);
                // change vector is now created, but it is empty
                // this can be used to detect newly created entities

                string changeVector = session.Advanced.GetChangeVectorFor(emp);
                Console.WriteLine($"Change vector for new entity    : '{changeVector}'");

                session.SaveChanges();
                // entity is saved to a database and change vector is generated and assigned

                changeVector = session.Advanced.GetChangeVectorFor(emp);
                Console.WriteLine($"Change vector for saved entity  : '{changeVector}'");

                var loadedEmp = session.Load<Employee>(emp.Id);
                changeVector = session.Advanced.GetChangeVectorFor(loadedEmp);
                Console.WriteLine($"Change vector for loaded entity : '{changeVector}'");

                loadedEmp.FirstName = "new_value";
                changeVector = session.Advanced.GetChangeVectorFor(loadedEmp);
                Console.WriteLine($"Change vector for changed entity: '{changeVector}'");

                session.SaveChanges();
                changeVector = session.Advanced.GetChangeVectorFor(loadedEmp);
                Console.WriteLine($"Change vector for saved entity  : '{changeVector}'");
            }
        }
    }
}
