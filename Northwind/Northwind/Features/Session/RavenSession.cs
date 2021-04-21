using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Session
{
    public class RavenSession
    {
        public void Evict()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee laura = session.Load<Employee>("employees/8-A");
            
            session.Advanced.Evict(laura);

            laura = session.Load<Employee>("employees/8-A");
            
            Console.WriteLine($"Total requests: {session.Advanced.NumberOfRequests}");
        }
    }
}
