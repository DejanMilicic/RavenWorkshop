using System.Collections.Generic;
using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.AdditionalSources
{
    // todo: rename the file

    public class Employees_Specials : AbstractIndexCreationTask<Employee, Employees_Specials.Entry>
    {
        public class Entry
        {
            public bool Special { get; set; }
        }

        public Employees_Specials()
        {
            Map = employees => from emp in employees
                            select new Entry
                            {
                                Special = EmployeeUtil.IsSpecial(emp.FirstName)
                            };

            AdditionalSources = new Dictionary<string, string>
            {
                {
                    "EmployeeUtil.cs",
                    @"
public static class EmployeeUtil
{
    public static bool IsSpecial(string name)
    {
        return name == ""Laura"";
    }
}
"
                }
            };
        }
    }
}
