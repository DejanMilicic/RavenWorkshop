using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Http;

namespace Northwind.Features.Client
{
    public class Client
    {
        public void Do()
        {
            //using var session = Dsh.Store.OpenSession();

            //var employees = session.Query<Employee>().Count();

            //Console.WriteLine($"Total employees: {employees}");

            //while (true)
            //{
            //    var sp = Stopwatch.StartNew();

            //    using (var session = Dsh.Store.OpenSession())
            //    {
            //        session.Store(new Employee());
            //        Thread.Sleep(1000);
            //        session.SaveChanges();
            //    }

            //    Console.WriteLine(sp.Elapsed);
            //}

            Random r = new Random();
            while (true)
            {
                using (var session = Dsh.Store.OpenSession())
                {
                    int id = r.Next(1, 830);

                    var order = session.Load<Order>($"orders/{id}-A");

                    Console.ReadLine();
                }
            }

        }
    }

    public static class Dsh
    {
        private static readonly Lazy<IDocumentStore> LazyStore =
            new Lazy<IDocumentStore>(() =>
            {
                var store = new DocumentStore
                {
                    Urls = new[]
                    {
                        "https://a.dejan.development.run/",
                        "https://b.dejan.development.run/",
                        "https://c.dejan.development.run/"
                    },
                    Certificate = new X509Certificate2(@"C:\temp\ravendb\admin.client.certificate.dejan.pfx"),
                    Database = "demo"
                };

                store.OnBeforeRequest += (sender, args) =>
                {
                    Console.WriteLine(args.Url);
                };

                store.Conventions.ReadBalanceBehavior = ReadBalanceBehavior.RoundRobin;

                store.Initialize();

                IndexCreation.CreateIndexes(typeof(Program).Assembly, store);

                return store;
            });

        public static IDocumentStore Store => LazyStore.Value;
    }
}
