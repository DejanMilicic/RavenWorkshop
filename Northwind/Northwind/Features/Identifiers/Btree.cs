using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using NUlid;
using NUlid.Rng;

namespace Northwind.Features.Identifiers
{
    // http://127.0.0.1:8080/databases/demo/debug/storage/btree-structure

    public class Btree
    {
        public void Balanced()
        {
            using var bulk = DocumentStoreHolder.Store.BulkInsert();

            for (int i = 0; i < 100 * 1000; i++)
            {
                bulk.Store(new Employee
                {
                    FirstName = "FirstName #" + i,
                    LastName = "LastName #" + i
                });
            }
        }

        public void Unbalanced()
        {
            using var bulk = DocumentStoreHolder.Store.BulkInsert();

            for (int i = 0; i < 100 * 1000; i++)
            {
                bulk.Store(new Employee
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "FirstName #" + i,
                    LastName = "LastName #" + i
                });
            }
        }

        public void Nulids()
        {
            var rng = new MonotonicUlidRng();

            using var bulk = DocumentStoreHolder.Store.BulkInsert();

            for (int i = 0; i < 100 * 1000; i++)
            {
                bulk.Store(new Employee
                {
                    Id = Ulid.NewUlid(rng).ToString(),
                    FirstName = "FirstName #" + i,
                    LastName = "LastName #" + i
                });
            }
        }

        public void RTcomb()
        {
            using var bulk = DocumentStoreHolder.Store.BulkInsert();

            for (int i = 0; i < 100 * 1000; i++)
            {
                bulk.Store(new Employee
                {
                    Id = RT.Comb.Provider.PostgreSql.Create().ToString(),
                    FirstName = "FirstName #" + i,
                    LastName = "LastName #" + i
                });
            }
        }
    }
}
