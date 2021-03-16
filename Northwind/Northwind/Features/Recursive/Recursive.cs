using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace Northwind.Features.Recursive
{
    public class Recursive
    {
        public void ManagerSearch()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            string managerName = "Andrew";

            var res = session.Advanced.RawQuery<Employee>(@$"
                    from index 'Employees/Managers' where managers = '{managerName}' 
                ").ToList();

            Console.WriteLine($"{managerName} manages \n");

            foreach (Employee employee in res)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName}");
            }
        }
    }
}
