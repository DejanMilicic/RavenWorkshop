using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Filter
{
    public static class Filter
    {
        public static void WithAutomaticIndex()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            List<Employee> emp = session.Query<Employee>()
                .Where(x => x.FirstName == "Nancy")
                .ToList();

            foreach (Employee employee in emp)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName}");
            }
        }

        public static void WithoutAutomaticIndex()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            List<Employee> emp = session.Query<Employee>()
                .Filter(x => x.FirstName == "Nancy")
                .Statistics(out QueryStatistics stats)
                .ToList();

            foreach (Employee employee in emp)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName}");
            }

            Console.WriteLine($"Scanned results: {stats.ScannedResults}");

        }
    }
}
