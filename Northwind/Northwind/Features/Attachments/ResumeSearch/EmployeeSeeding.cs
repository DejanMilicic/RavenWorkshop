using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Raven.Client.Documents;

namespace Northwind.Features.Attachments.ResumeSearch
{
    public static class Employees
    {
        public static void Seed()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var emp = new Employee()
            {
                FirstName = "Jonathan",
                LastName = "Doe"
            };
            session.Store(emp);

            using FileStream cv = File.Open("./Features/Attachments/ResumeSearch/cv.txt", FileMode.Open);
            session.Advanced.Attachments.Store(emp.Id, "cv.txt", cv, "text/plain");

            session.SaveChanges();
        }

        public static void Search()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            string term = "Netflix";

            List<Employee> employees = session
                .Advanced
                .DocumentQuery<Employee, Employees_Search>()
                .Search("Content", term)
                .ToList();

            Console.WriteLine($"Search results for: {term}");
            Console.WriteLine("===");
            foreach (Employee emp in employees)
            {
                Console.WriteLine($"> {emp.FirstName} {emp.LastName}");
            }
        }
    }
}
