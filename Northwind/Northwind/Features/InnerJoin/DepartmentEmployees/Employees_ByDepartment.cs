using Raven.Client.Documents.Indexes;
using System.Linq;

namespace Northwind.Features.InnerJoin.DepartmentEmployees;

public class Employees_ByDepartment : AbstractMultiMapIndexCreationTask<Employees_ByDepartment.Entry>
{
    public class Entry
    {
        public string[] EmployeeId { get; set; }

        public string StoreId { get; set; }

        public string DepartmentId { get; set; }
    }

    public Employees_ByDepartment()
    {
        AddMap<Department>(departments =>
            from department in departments
            from storeId in department.StoreIds
            select new Entry
            {
                EmployeeId = new string[] {},
                StoreId = storeId,
                DepartmentId = department.Id
            });

        AddMap<Employee>(employees =>
            from employee in employees
            select new Entry
            {
                EmployeeId = new string[] { employee.Id },
                StoreId = employee.StoreId,
                DepartmentId = ""
            });

        Reduce = results =>
            from result in results
            group result by new
            {
                result.StoreId
            }
            into g
            select new Entry
            {
                StoreId = g.Key.StoreId,
                EmployeeId = g.SelectMany(x => x.EmployeeId).ToArray(),
                DepartmentId = g.First(x => x.DepartmentId != "").DepartmentId
            };
    }
}

