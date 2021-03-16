using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents.Queries.Timings;

namespace Northwind.Features.Timings
{
    public class Timings
    {
        public void GetTimings()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var employees = session.Advanced.DocumentQuery<Employee>()
                .Timings(out QueryTimings timings)
                .WhereEquals(x => x.Address.Country, "UK")
                .ToList();

            Console.WriteLine($"{employees.Count} employees fetched in {timings.DurationInMs}ms");
        }
    }
}
