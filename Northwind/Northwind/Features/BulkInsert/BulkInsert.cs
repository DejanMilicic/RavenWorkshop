using System;
using Northwind.Models.Entity;

namespace Northwind.Features.BulkInsert
{
    public class BulkInsert
    {
        public void Do()
        {
             // todo: introduce timer so we can see how long it lasts
             // todo: comparison with inserting via session

            using var bulk = DocumentStoreHolder.Store.BulkInsert();

            for (int i = 0; i < 1000 * 1000; i++)
            {
                bulk.Store(new Employee
                {
                    FirstName = "FirstName #" + i,
                    LastName = "LastName #" + i
                });
            }
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
