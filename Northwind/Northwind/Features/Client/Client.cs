﻿using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Operations.CompareExchange;
using Raven.Client.Http;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Client
{
    public class Client
    {
        public void ClientFailover()
        {
            using var session = Dsh.Store.OpenSession();

            var employees = session.Query<Employee>().Count();

            Console.WriteLine($"Total employees: {employees}");
        }

        public void ClientFailover2()
        {
            while (true)
            {
                var sp = Stopwatch.StartNew();

                using (var session = Dsh.Store.OpenSession())
                {
                    session.Store(new Employee());
                    Thread.Sleep(1000);
                    session.SaveChanges();
                }

                Console.WriteLine(sp.Elapsed);
            }
        }

        public void RoundRobinFastestNodeDemo()
        {
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

        public void SessionContext()
        {
            string ctx = "1";

            while (true)
            {
                using (var session = Dsh.Store.OpenSession())
                {
                    session.Advanced.SessionInfo.SetContext(ctx);

                    session.Query<Employee>().Where(x => x.FirstName == "Roger").ToList();

                    session.Store(new { });
                    session.SaveChanges();
                }

                ctx = Console.ReadLine() ?? "1";
                if (string.IsNullOrWhiteSpace(ctx)) ctx = "1";
            }
        }

        public void CompareExchange()
        {
            var operation = new PutCompareExchangeValueOperation<string>("dejan@ravendb.net", "users/1-A", 0);

            CompareExchangeResult<string> result = Dsh.Store.Operations.Send(operation);
            Console.WriteLine(result.Successful);

            var val = Dsh.Store.Operations.Send(
                new GetCompareExchangeValueOperation<string>("dejan@ravendb.net"));
        }

        public void CompareExchange2()
        {
            using var session = Dsh.Store.OpenSession();

            var user = new User
            {

                Email = "dejan@ravendb.net"
            };

            session.Store(user);

            var cmpXchgResult = Dsh.Store.Operations.Send(
                    new PutCompareExchangeValueOperation<string>(user.Email, user.Id, 0));

            if (cmpXchgResult.Successful == false)
                throw new Exception("Email is already in use");

            // this code is not safe... what happens if execution never reaches next line?
            // "reserved" email will be still reserved, and document will not be created

            session.SaveChanges();

            // from Users as u
            // include CmpXchg(u.Email)

            // from Users
            // where id() = CmpXchg("dejan@ravendb.net")

            // from @all_docs 
            // where id() = CmpXchg("dejan@ravendb.net")
        }

        public void ClusterWideTransaction()
        {
            using (var session = Dsh.Store.OpenSession(new SessionOptions
            {
                //default is:     TransactionMode.SingleNode
                TransactionMode = TransactionMode.ClusterWide
            }))
            {
                var user = new Employee
                {
                    FirstName = "John",
                    LastName = "Doe"
                };
                session.Store(user);

                // this transaction is now conditional on this being 
                // successfully created (so, no other users with this name)
                // it also creates an association to the new user's id
                session.Advanced.ClusterTransaction
                    .CreateCompareExchangeValue("usernames/John", user.Id);

                session.SaveChanges();
            }
        }

        public void ClusterWideTransaction2()
        {
            using var session = Dsh.Store.OpenSession(new SessionOptions
            {
                TransactionMode = TransactionMode.ClusterWide
            });

            var user = new Employee { FirstName = "Dejan" };
            session.Store(user); // hilo
            session.Advanced.ClusterTransaction.CreateCompareExchangeValue("dejan@ravendb.net", user.Id);
            session.SaveChanges();
        }
    }

    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
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
