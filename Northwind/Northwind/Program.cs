using System;
using System.Security.Cryptography.X509Certificates;
using Northwind.Models.Entity;
using Raven.Client.Documents;

namespace Northwind
{
    class Program
    {
        static void Main(string[] args)
        {
            using var store = new DocumentStore
            {
                Urls = new string[] { "https://a.d2.development.run/" },
                Certificate = new X509Certificate2("admin.client.certificate.d2.pfx"),
                Database = "demo"
            }.Initialize();

            #region 1. Read entity from the database

            //using var session = store.OpenSession();
            //Employee employee = session.Load<Employee>("employees/8-A");
            //Console.WriteLine(employee.FirstName);

            #endregion

            #region 2. Modify entity and save it to the database

            //using var session = store.OpenSession();
            //Employee employee = session.Load<Employee>("employees/8-A");
            //employee.LastName = employee.LastName.ToUpper();
            //session.SaveChanges();

            #endregion

            #region 3. Counters

            //using var session = store.OpenSession();
            //var counters = session.CountersFor("products/74-A");

            //string twoStars = counters.Get("⭐⭐").ToString();
            //Console.WriteLine(twoStars);

            //counters.Increment("⭐⭐");
            //counters.Increment("⭐⭐⭐", -1);
            //session.SaveChanges();

            #endregion
        }
    }
}
