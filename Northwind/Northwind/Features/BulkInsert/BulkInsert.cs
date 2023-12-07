using System;
using System.Diagnostics;
using Northwind.Models.Entity;
using Raven.Client.Documents.BulkInsert;

namespace Northwind.Features.BulkInsert
{
    public static class BulkInsert
    {
        public static void DoViaSession()
        {
            var timer = new Stopwatch();
            timer.Start();

            for (int i = 0; i < 25_000; i++)
            {
                using (var session = DocumentStoreHolder.Store.OpenSession())
                {
                    session.Store(new Employee
                    {
                        FirstName = "FirstName #" + i,
                        LastName = "LastName #" + i
                    });

                    session.SaveChanges();
                }
            }

            timer.Stop();
            Console.WriteLine($"Elapsed: " + timer.Elapsed.ToString(@"m\:ss\.fff"));
        }

        public static void Do()
        {
            var timer = new Stopwatch();
            timer.Start();

            using BulkInsertOperation bulk = DocumentStoreHolder.Store.BulkInsert();

            for (int i = 0; i < 40 * 25_000; i++)
            {
                bulk.Store(new Employee
                {
                    FirstName = "FirstName #" + i,
                    LastName = "LastName #" + i
                });
            }

            timer.Stop();
            Console.WriteLine($"Elapsed: " + timer.Elapsed.ToString(@"m\:ss\.fff"));
        }

        // todo : add example of not overriding existing ones
        // todo : read/determine size of the batch

        public static void DoWithIds()
        {
            using var bulk = DocumentStoreHolder.Store.BulkInsert();

            for (int i = 0; i < 100; i++)
            {
                string id = bulk.Store(new Employee
                        {
                            FirstName = "FirstName #" + i,
                            LastName = "LastName #" + i
                        });

                Console.WriteLine(id);
            }
        }
    }
}
