using System;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Counters
{
    public class Counters
    {
        public void GetCounters()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            ISessionDocumentCounters counters = session.CountersFor("products/77-A");

            string twoStars = counters.Get("⭐⭐").ToString();
            
            Console.WriteLine(twoStars);
        }

        public void UpdateCounters()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            ISessionDocumentCounters counters = session.CountersFor("products/77-A");

            counters.Increment("⭐⭐");
            counters.Increment("⭐⭐⭐", -1);
            
            session.SaveChanges();
        }

        public void CreateCounter()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            session.CountersFor("employees/8-A").Increment("Likes", 1);
            
            session.SaveChanges();
        }

        public async Task ConcurrentUpdates()
        {
            for (int i = 0; i < 100; i++)
            {
                await Task.Factory.StartNew(async () =>
                  {
                      using var session = DocumentStoreHolder.Store.OpenAsyncSession();
                      session.CountersFor("employees/8-A").Increment("Likes");
                      await session.SaveChangesAsync();
                  });
            }
        }
    }
}
