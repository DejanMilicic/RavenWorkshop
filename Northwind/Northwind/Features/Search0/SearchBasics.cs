using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Features.Attachments.ResumeSearch;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Queries.Highlighting;
using Employee = Northwind.Models.Entity.Employee;

namespace Northwind.Features.Search0
{
    internal static class SearchBasics
    {
        internal static void Execute()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            List<Employee> employees = session.Query<Employee>()
                .Search(x => x.Notes, "french")
                .ToList();

            PrintEmployees("French-speaking employees", employees);

            employees = session.Query<Employee>()
                .Search(x => x.Notes, "Washington Colorado")
                .ToList();

            PrintEmployees("Employees related to Washington OR Colorado", employees);

            employees = session.Query<Employee>()
                .Search(x => x.Notes, "Spanish Portuguese", @operator: SearchOperator.And)
                .Search(x => x.Notes, "Manager", options: SearchOptions.Or)
                .ToList();

            PrintEmployees("Employees speaking (Spanish AND Portuguese) OR Manager", employees);

            List<Product> products = session.Query<Product>()
                .Search(x => x.Name, "tofu")
                .ToList();

            PrintProducts("Tofu products", products);

            products = session.Query<Product>()
                .Search(x => x.Name, "ch*")
                .ToList();

            PrintProducts("Ch* products", products);

            products = session.Query<Product>()
                .Search(x => x.Name, "*ing")
                .ToList();

            PrintProducts("*ing products", products);

            products = session.Query<Product>()
                .Search(x => x.Name, "*ad*")
                .ToList();

            PrintProducts("*ad* products", products);

            employees = session.Query<Employee>()
                .Search(x => x.Notes, "ph.d.", boost: 100)
                .Search(x => x.Notes, "university", boost: 20)
                .Search(x => x.Notes, "college", boost: 5)
                .ToList();

            PrintEmployees("Employees, by education, with rank boosting", employees);

            employees = session.Query<Employee>()
                .Highlight("Notes", 50, 1, out Highlightings notesHighlightings)
                .Search(x => x.Notes, "sales")
                .OfType<Employee>()
                .ToList();

            PrintEmployeesHighlights("Employees in sales, with highlights", employees, notesHighlightings);
        }

        private static void PrintEmployees(string title, List<Employee> employees)
        {
            Console.WriteLine($"\n{title}\n");

            int counter = 1;
            foreach (Employee employee in employees)
            {
                Console.WriteLine($"{counter++}. {employee.FirstName} {employee.LastName}");
            }
        }

        private static void PrintEmployeesHighlights(string title, List<Employee> employees, Highlightings notesHighlightings)
        {
            Console.WriteLine($"\n{title}\n");

            for (int i = 0; i < employees.Count; i++)
            {
                Employee emp = employees[i];

                Console.WriteLine($"{i+1}. {emp.FirstName} {emp.LastName}");
                Console.WriteLine(String.Join(" ", notesHighlightings.GetFragments(emp.Id)));
            }
        }

        private static void PrintProducts(string title, List<Product> products)
        {
            Console.WriteLine($"\n{title}\n");

            int counter = 1;
            foreach (Product product in products)
            {
                Console.WriteLine($"{counter++}. {product.Name}");
            }
        }
    }
}
