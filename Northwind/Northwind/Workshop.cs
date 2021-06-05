using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Operations.CompareExchange;
using Raven.Client.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Northwind.Features.Caching;
using Raven.Client.Documents.Commands;
using Raven.Client.Documents.Operations.Revisions;
using Raven.Client.Documents.Session;

namespace Northwind
{
    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
    }

    public class Workshop
    {
        public void Do()
        {
            //using var session = Store.OpenSession(new SessionOptions {TransactionMode = TransactionMode.ClusterWide});

            //var command = new GetDocumentsCommand("orders/830-A", null, false);
            //session.Advanced.RequestExecutor.Execute(command, session.Advanced.Context);
            //var result = command.Result.Results.FirstOrDefault();
            //var json = result?.ToString();



            //Employee laura = session.Load<Employee>("employees/8-a");
            //laura.LastName = "Smith";
            //session.SaveChanges();



            //var operation = new PutCompareExchangeValueOperation<string>(user.Email, user.Id, 0);
            //CompareExchangeResult<string> result = Store.Operations.Send(operation);

            //if (result.Successful == false)
            //    throw new Exception("Email is already in use");

            using var session = Store.OpenSession();

            var results = session.Query<Order>()
                .GroupBy(x => new
                {
                    x.Employee,
                    x.Company
                })
                .Select(x => new
                {
                    EmployeeIdentifier = x.Key.Employee,
                    x.Key.Company,
                    Count = x.Count()
                })
                .ToList();

        }


        public static IDocumentStore Store => LazyStore.Value;

        private static readonly Lazy<IDocumentStore> LazyStore =
            new Lazy<IDocumentStore>(() =>
            {
                var store = new DocumentStore
                {
                    Urls = new[]
                    {
                        "http://live-test.ravendb.net"
                    },
                    //Certificate = new X509Certificate2(@"C:\temp\admin.client.certificate.dejanm.pfx"),
                    Database = "demo"
                };

                //store.OnBeforeRequest += (sender, args) =>
                //{
                //    Console.WriteLine(args.Url);
                //};

                var configuration = new RevisionsConfiguration
                {
                    Default = new RevisionsCollectionConfiguration
                    {
                        Disabled = false,
                        MinimumRevisionsToKeep = null
                    },
                    Collections = new Dictionary<string, RevisionsCollectionConfiguration>
                    {
                        ["Users"] = new RevisionsCollectionConfiguration
                        {
                            Disabled = false,
                            PurgeOnDelete = true,
                            MinimumRevisionsToKeep = 123
                        },
                        ["People"] = new RevisionsCollectionConfiguration
                        {
                            Disabled = false,
                            MinimumRevisionsToKeep = 10
                        },
                        ["Comments"] = new RevisionsCollectionConfiguration
                        {
                            Disabled = true
                        },
                        ["Products"] = new RevisionsCollectionConfiguration
                        {
                            Disabled = true
                        }
                    }
                };

                store.Maintenance.Send(new ConfigureRevisionsOperation(configuration));

                store.Conventions.ReadBalanceBehavior = ReadBalanceBehavior.RoundRobin;

                store.Initialize();

                IndexCreation.CreateIndexes(typeof(Program).Assembly, store);

                return store;
            });
    }
}
