using Northwind.Models.Entity;

namespace Northwind.Features.BulkInsert
{
    public class BulkInsert
    {
        public void Do()
        {
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
    }
}
