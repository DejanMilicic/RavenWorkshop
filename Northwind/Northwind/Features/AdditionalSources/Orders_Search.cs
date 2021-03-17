using System;
using System.Collections.Generic;
using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace Northwind.Features.Indexes
{
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
