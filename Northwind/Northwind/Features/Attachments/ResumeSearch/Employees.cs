using System;
using System.Collections.Generic;
using System.IO;

namespace Northwind.Features.Attachments.ResumeSearch
{
    public static class Employees
    {
        public static void Seed()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            #region John Doe

            var johnDoe = new Employee()
            {
                FirstName = "Jonathan",
                LastName = "Doe"
            };
            session.Store(johnDoe);

            using FileStream johnCv = File.Open("./Features/Attachments/ResumeSearch/cv.txt", FileMode.Open);
            session.Advanced.Attachments.Store(johnDoe.Id, "cv.txt", johnCv, "text/plain");

            #endregion

            #region Vlad Dracula

            var vladDracula = new Employee()
            {
                FirstName = "Vlad",
                LastName = "Dracula"
            };
            session.Store(vladDracula);

            using FileStream draculaCv = File.Open("./Features/Attachments/ResumeSearch/dracula.zip", FileMode.Open);
            session.Advanced.Attachments.Store(vladDracula.Id, "dracula.zip", draculaCv, "application/zip");

            #endregion

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
