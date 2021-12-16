using System;
using System.Diagnostics;
using Northwind.Models.Entity;

namespace Northwind.Features.BulkInsert
{
    public class BulkInsert
    {
        // todo: comparison with inserting via session

        public void Do()
        {
            var timer = new Stopwatch();
            timer.Start();

            using var bulk = DocumentStoreHolder.Store.BulkInsert();

            for (int i = 0; i < 1000 * 1000; i++)
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

        public void DoWithIds()
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
