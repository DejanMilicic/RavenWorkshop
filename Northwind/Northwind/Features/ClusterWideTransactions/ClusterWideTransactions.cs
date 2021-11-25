using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;

namespace Northwind.Features.ClusterWideTransactions
{
    public class ClusterWideTransactions
    {
        // ensure there is just one user with first name John
        // execute this method once and inspect database
        // execute it one more time to get exception
        public void ClusterWideTransaction_Pre_5_2()
        {
            using var session = DocumentStoreHolder.Store.OpenSession(
                new SessionOptions
                {
                    TransactionMode = TransactionMode.ClusterWide,
                    DisableAtomicDocumentWritesInClusterWideTransaction = true // pre-5.2 mode
                });

            Employee emp = new Employee { FirstName = "John", LastName = "Smith" };

            session.Store(emp);
            session.Advanced.ClusterTransaction.CreateCompareExchangeValue($"users/{emp.FirstName}", "");

            try
            {
                session.SaveChanges();
            }
            catch (ConcurrencyException)
            {
                Console.WriteLine($"User with first name {emp.FirstName} already exists in the database");
            }
        }
    }
}
