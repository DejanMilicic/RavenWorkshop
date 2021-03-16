using System.Collections.Generic;
using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Recursive
{
    public class Employees_Managers : AbstractIndexCreationTask<Employee, Employees_Managers.Entry>
    {
        public class Entry
        {
            public IEnumerable<string> Managers { get; set; }
        }

        public Employees_Managers()
        {
            Map = employees => from e in employees
                let managersOfe = Recurse(
                    LoadDocument<Employee>(e.ReportsTo),
                    empl => LoadDocument<Employee>(empl.ReportsTo)
                    )
                select new
                {
                    managers = managersOfe.Select(x => x.FirstName)
                };
        }
    }
}
