using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Operations.CompareExchange;
using Raven.Client.Http;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Transactions;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;

namespace Northwind.Features.Client
{
    public class Client
    {
        public void PreferredNode()
        {
            // showcase preferred node getting hits

            var store = DocumentStoreHolder.GetStore();
            store.OnBeforeRequest += (sender, args) => Console.WriteLine(args.Url);
            store.Initialize();
            
            using var session = store.OpenSession();

            var employees = session.Query<Employee>().Count();

            Console.WriteLine($"Total employees: {employees}");
        }

        public void ClientFailoverRead()
        {
            // showcase what happens when you kill preferred node
            // showcase round robin

            var store = DocumentStoreHolder.GetStore();
            store.OnBeforeRequest += (sender, args) => Console.WriteLine(args.Url);
            store.Conventions.ReadBalanceBehavior = ReadBalanceBehavior.RoundRobin;
            store.Initialize();

            while (true)
            {
                var sp = Stopwatch.StartNew();

                using (var session = store.OpenSession())
                {
                    session.Load<Order>("orders/1-A");
                    Thread.Sleep(1000);
                }

                Console.WriteLine(sp.Elapsed);
            }
        }

        public void ClientFailoverWrite()
        {
            var store = DocumentStoreHolder.GetStore();
            store.OnBeforeRequest += (sender, args) => Console.WriteLine(args.Url);
            store.Initialize();

            while (true)
            {
                var sp = Stopwatch.StartNew();

                using (var session = store.OpenSession())
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
            var store = DocumentStoreHolder.GetStore();
            store.OnBeforeRequest += (sender, args) => Console.WriteLine(args.Url);
            store.Conventions.ReadBalanceBehavior = ReadBalanceBehavior.RoundRobin;
            store.Initialize();

            Random r = new Random();
            while (true)
            {
                using (var session = store.OpenSession())
                {
                    int id = r.Next(1, 830);

                    var order = session.Load<Order>($"orders/{id}-A");

                    Console.ReadLine();
                }
            }
        }

        public void SessionContext()
        {
            var store = DocumentStoreHolder.GetStore();
            store.OnBeforeRequest += (sender, args) => Console.WriteLine(args.Url);
            store.Initialize();

            string ctx = "1";

            while (true)
            {
                using (var session = store.OpenSession())
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
            var store = DocumentStoreHolder.GetStore();

            var operation = new PutCompareExchangeValueOperation<string>("dejan@ravendb.net", "users/1-A", 0);

            CompareExchangeResult<string> result = store.Operations.Send(operation);
            Console.WriteLine($"Compare Exchange creation: {result.Successful}");

            var val = store.Operations.Send(
                new GetCompareExchangeValueOperation<string>("dejan@ravendb.net"));
        }

        public void CompareExchange2()
        {
            var store = DocumentStoreHolder.GetStore();

            using var session = store.OpenSession();

            var user = new User
            {
                Email = "dejan@ravendb.net"
            };

            session.Store(user);

            var cmpXchgResult = store.Operations.Send(
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
            var store = DocumentStoreHolder.GetStore();

            using var session = store.OpenSession(new SessionOptions
            {
                //default is:     TransactionMode.SingleNode
                TransactionMode = TransactionMode.ClusterWide
            });

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

        public void ClusterWideTransaction2()
        {
            var store = DocumentStoreHolder.GetStore();

            using var session = store.OpenSession(new SessionOptions
            {
                TransactionMode = TransactionMode.ClusterWide
            });

            var user = new Employee { FirstName = "Dejan" };
            session.Store(user); // hilo-powered node-bound id generated
            session.Advanced.ClusterTransaction.CreateCompareExchangeValue("dejan@ravendb.net", user.Id);
            session.SaveChanges();
        }

        public void ClusterWideTransactionRavenDb_5_2_plus()
        {
            var store = DocumentStoreHolder.GetStore();

            using var session = store.OpenSession(new SessionOptions
            {
                TransactionMode = TransactionMode.ClusterWide
            });

            var user = new Employee { FirstName = "Dejan" };
            session.Store(user); // hilo-powered node-bound id generated
            session.Store(new { ReservedFor = user.Id }, "Employees/dejan@ravendb.net");
            session.SaveChanges();
        }

        public void SaveSameDocumentAgain()
        {
            var store = DocumentStoreHolder.GetStore().Initialize();

            using var session = store.OpenSession();

            var user = new Employee { Id = "Employees/Marco", FirstName = "Marco" };
            session.Store(user);
            Console.WriteLine(user.Id);
            session.SaveChanges();

            session.Advanced.Clear();

            var user2 = new Employee { Id = "Employees/Marco", FirstName = "Marco 2" };
            session.Store(user2);
            Console.WriteLine(user2.Id);
            session.SaveChanges();
        }

        public void SaveSameDocumentAgain_WithOptimisticConcurrency()
        {
            var store = DocumentStoreHolder.GetStore().Initialize();

            using var session = store.OpenSession();
            session.Advanced.UseOptimisticConcurrency = true;

            var user = new Employee { Id = "Employees/Marco", FirstName = "Marco" };
            session.Store(user);
            Console.WriteLine(user.Id);
            session.SaveChanges();

            session.Advanced.Clear();

            var user2 = new Employee { Id = "Employees/Marco", FirstName = "Marco 2" };
            session.Store(user2);

            try
            {
                session.SaveChanges();
            }
            catch (ConcurrencyException)
            {
                Console.WriteLine($"Error: Document with Id='{user2.Id}' already exists");
            }
        }
    }

    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
    }
}
