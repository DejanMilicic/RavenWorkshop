using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions.Equivalency;
using Raven.Client.Documents;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Queries.Highlighting;
using Spectre.Console;
using Employee = Northwind.Models.Entity.Employee;
using Product = Northwind.Models.Entity.Product;

namespace Northwind.Features.SearchBasic
{
    public static class SearchBasics
    {
        public static void Execute()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            // Search for all employees
            // with occurrences of "french" within property Notes
            /*
                from 'Employees' 
                where search(Notes, "french")
             */
            List<Employee> employees = session.Query<Employee>()
                .Search(x => x.Notes, "french")
                .ToList();

            PrintEmployees("French-speaking employees", employees);


            // Search for all employees
            // with occurrences of terms "Washington" or "Colorado"
            /*
                from 'Employees' 
                where search(Notes, "Washington Colorado")
             */
            // operator "or" is implicit, so this is identical to
            /*
                from 'Employees'
                where search(Notes, "Washington Colorado", or)
             */
            employees = session.Query<Employee>()
                .Search(x => x.Notes, "Washington Colorado")
                // identical to
                //.Search(x => x.Notes, "Washington Colorado", @operator: SearchOperator.Or)
                .ToList();

            PrintEmployees("Employees related to Washington OR Colorado", employees);


            // Search for all employees
            // with both "Spanish" and "Portuguese" in their Notes
            // or "Manager" in the Notes
            /*
                from 'Employees'
                where 
                    search(Notes, "Spanish Portuguese", and) 
                    or search(Notes, "Manager")
            */
            employees = session.Query<Employee>()
                .Search(x => x.Notes, "Spanish Portuguese", @operator: SearchOperator.And)
                .Search(x => x.Notes, "Manager", options: SearchOptions.Or)
                .ToList();

            PrintEmployees("Employees speaking (Spanish AND Portuguese) OR Manager", employees);


            /*
                from 'Products' 
                where search(Name, "tofu")
             */
            List<Product> products = session.Query<Product>()
                .Search(x => x.Name, "tofu")
                .ToList();

            PrintProducts("Tofu products", products);


            /*
                from 'Products'
                where search(Name, "ch*")
             */
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

            // search with regular expression
            products = session
                .Query<Product>()
                .Where(x => Regex.IsMatch(x.Name, "^[C-F][a-zA-Z]*$"))
                .ToList();

            PrintProducts("Products with a single word name, starting with letters C-F", products);
        }

        private static void PrintEmployees(string title, List<Employee> employees)
        {
            AnsiConsole.Markup($"\n[black on yellow]{title}[/]\n\n");

            var grid = new Grid();
            grid.AddColumn();
            foreach (Employee employee in employees)
                grid.AddRow($" {employee.FirstName} {employee.LastName}");

            AnsiConsole.Write(grid);
        }

        private static void PrintEmployeesHighlights(string title, List<Employee> employees, Highlightings notesHighlightings)
        {
            AnsiConsole.Markup($"\n[black on yellow]{title}[/]\n\n");

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();

            foreach (Employee emp in employees)

                grid.AddRow($" {emp.FirstName} {emp.LastName}",
                    String.Join(" ", notesHighlightings.GetFragments(emp.Id)));

            AnsiConsole.Write(grid);
        }

        private static void PrintProducts(string title, List<Product> products)
        {
            AnsiConsole.Markup($"\n[black on yellow]{title}[/]\n\n");

            var grid = new Grid();
            grid.AddColumn();
            foreach (Product product in products)
                grid.AddRow($" {product.Name}");
            
            AnsiConsole.Write(grid);
        }
    }
}
