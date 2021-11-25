using System;
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

        // ensure there is just one user with first name John
        // execute this method once and inspect database
        // execute it one more time to get exception
        public void ClusterWideTransaction()
        {
            using var session = DocumentStoreHolder.Store.OpenSession(
                new SessionOptions { TransactionMode = TransactionMode.ClusterWide });

            Employee emp = new Employee { FirstName = "John", LastName = "Smith" };

            session.Store(emp);
            // this document will be stored in @empty collection
            // it acts as a "reservations" tracking mechanism to ensure uniqueness of first name
            // since it will result in creation of CMPXCHG entry with a key "rvn-atomic/firstnames/john"
            session.Store(new { TakenBy = emp.Id }, $"firstnames/{emp.FirstName}");
            
            try
            {
                session.SaveChanges();
            }
            catch (ConcurrencyException)
            {
                Console.WriteLine($"User with first name {emp.FirstName} already exists in the database");
            }

            // results of execution
            // two documents are saved
            // two CMPXCHG entries will be created, based on IDs of saved documents
            //
            // rvn-atomic/employees/1-a
            // rvn-atomic/firstnames/john
            //
            // second one serves as a "reservation" of the firstname, to ensure uniqueness
            //
            // both documents' Change Vectors are augmented with the same TRXN
            // since they are both saved in the same cluster-wide transaction
            // RAFT:5-vgzU78+OUUCbw9JaV2OB5g,TRXN:636-1Yq/AQoPXEyGqFRmhLn9RA
            // RAFT:6-vgzU78+OUUCbw9JaV2OB5g,TRXN:636-1Yq/AQoPXEyGqFRmhLn9RA
        }
    }
}
