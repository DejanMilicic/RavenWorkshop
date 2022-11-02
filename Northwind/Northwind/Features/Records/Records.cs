using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Features.Records
{
    public static class Records
    {
        public static void Create()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Receipt receipt = new Receipt
            {
                Store = "My local store",
                Description = "Shoes",
                Amount = 199.99m,
                PurchaseDate = DateTime.UtcNow
            };

            session.Store(receipt);
            session.SaveChanges();
        }

        public static void Update()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Receipt receipt = session.Query<Receipt>().First();

            session.Advanced.Evict(receipt);
            session.Store(receipt with { Store = "Big store" });
            
            session.SaveChanges();
        }
    }
}
