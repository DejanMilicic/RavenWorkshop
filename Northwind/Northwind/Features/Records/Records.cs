using System;
using System.Linq;
using Raven.Client.Documents.Session;

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

        public static void Update2()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Receipt receipt = session.Query<Receipt>()
                .Customize(x => x.NoTracking())
                .First();

            session.Store(receipt with { Store = "Big store" });
            
            session.SaveChanges();
        }

        // this will not work - you cannot call Store for non-tracked session
        public static void Update3()
        {
            using var session = DocumentStoreHolder.Store.OpenSession(
                    new SessionOptions{ NoTracking = true });

            Receipt receipt = session.Query<Receipt>().First();

            session.Store(receipt with { Store = "Big store" });
            
            session.SaveChanges();
        }
    }
}
