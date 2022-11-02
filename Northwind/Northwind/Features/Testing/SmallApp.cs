using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents;

namespace Northwind.Features.Testing
{
    public class SmallApp
    {
        private readonly IDocumentStore Store;

        // poor man's dependency injection
        public SmallApp(IDocumentStore store)
        {
            Store = store;
        }

        public void CreateNewEmployee(Employee emp)
        {
            using var session = Store.OpenSession();
            session.Store(emp);
            session.SaveChanges();
        }
    }
}
