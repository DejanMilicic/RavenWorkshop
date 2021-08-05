using System;
using Northwind.Models.Entity;

namespace Northwind.Features.Migrations
{
    public class Migrations
    {
        public void MissingProperty()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Category seafood = session.Load<Category>("categories/8-A");
            seafood.Name += " 2";
            session.SaveChanges();
            
            Console.WriteLine($"Total requests: {session.Advanced.NumberOfRequests}");
        }
    }
}
