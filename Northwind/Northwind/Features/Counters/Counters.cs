using System;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Counters
{
    public class Counters
    {
        public void GetCounters()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            ISessionDocumentCounters counters = session.CountersFor("products/74-A");

            string twoStars = counters.Get("⭐⭐").ToString();
            
            Console.WriteLine(twoStars);
        }




        //counters.Increment("⭐⭐");
        //counters.Increment("⭐⭐⭐", -1);
        //session.SaveChanges();
    }
}
