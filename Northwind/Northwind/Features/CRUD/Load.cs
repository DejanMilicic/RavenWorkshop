using System.Collections.Generic;
using Northwind.Models.Entity;

namespace Northwind.Features.CRUD
{
    internal static class Load
    {
        internal static void NonexistingId()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee employee = session.Load<Employee>("nonexisting/id");

            // loading document by id that does not exist in the database
            // employee object will be null
        }

        internal static void NonexistingIdEnumerable()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Dictionary<string, Employee> employees = session.Load<Employee>(new []{ "nonexisting/id"});

            // loading document by nonexisting id
            // employee dict contains one entry
            //
            // <"noneisting/id", null>
        }

        internal static void ExistingNonexistingMix()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Dictionary<string, Employee> employees = session.Load<Employee>(
                new []{ "nonexisting/id", "employees/8-a"});

            // loading document by nonexisting id AND existing id
            // employee dict contains two entries
            //
            // <"noneisting/id", null>
            // <"employees/8-a", object>
        }
    }
}
