using Raven.Client.Documents.Indexes;
using Raven.Client.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.InnerJoin.DepartmentEmployees;

public static class DepartmentsEmployees
{
    public static void Demo()
    {
        var store = new DocumentStore
        {
            Urls = new[] { "http://127.0.0.1:8080" },
            Database = "Company"
        }.Initialize();

        IndexCreation.CreateIndexes(new AbstractIndexCreationTask[] { new Employees_ByDepartment() }, store);

        // uncomment to seed
        //Seed.Data(store);

        using var session = store.OpenSession();

        List<Employees_ByDepartment.Entry> entries = session.Query<Employees_ByDepartment.Entry, Employees_ByDepartment>().ToList();

        Console.WriteLine("All Employees");

        foreach (Employees_ByDepartment.Entry entry in entries)
        {
            foreach (string employee in entry.EmployeeId)
            {
                Console.WriteLine($"Employee: {employee}, Store: {entry.StoreId}, Department: {entry.DepartmentId}");
            }
        }

        Console.WriteLine("\n\nAll Employees working at Departments/NorthAmerica");

        List<Employees_ByDepartment.Entry> northAmerica = 
            session.Query<Employees_ByDepartment.Entry, Employees_ByDepartment>()
                .Where(x => x.DepartmentId == "Departments/NorthAmerica")
                .ToList();

        foreach (Employees_ByDepartment.Entry entry in northAmerica)
        {
            foreach (string employee in entry.EmployeeId)
            {
                Console.WriteLine($"Employee: {employee}, Store: {entry.StoreId}, Department: {entry.DepartmentId}");
            }
        }
    }
}

