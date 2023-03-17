using Raven.Client.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Northwind.Features.BulkInsert.RawJson
{
    public static class BulkInsertRawJson
    {
        private static IDocumentStore GetStore()
        {
            var store = new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "portfolio",
                Conventions =
                {
                    BulkInsert =
                    {
                        TrySerializeEntityToJsonStream = (entity, metadata, writer) =>
                        {
                            writer.Write((string)entity);
                            return true;
                        }
                    }
                }
            };

            store.Initialize();

            return store;
        }

        public static void Demo()
        {
            Console.WriteLine("Starting seeding...");
            var portfolios = Seeder.CreateJsonPortfolio();
            Console.WriteLine("Seeding completed\n");

            IDocumentStore store = GetStore();

            // here to force a request for RavenDB, nothing else. So the benchmark won't have to create
            // the connection to the server, we can assume that this is already there
            store.Maintenance.Send(new Raven.Client.Documents.Operations.GetStatisticsOperation());

            Console.WriteLine("Starting insert...");

            var sp = Stopwatch.StartNew();

            var tasks = new List<Task>();

            foreach (var chunk in portfolios.Chunk(1000))
            {
                tasks.Add(
                    Task.Run(async () =>
                        {
                            await using var bulk = store.BulkInsert();
                            foreach (var p in chunk)
                            {
                                await bulk.StoreAsync(p.Json, p.Id);
                            }
                        }
                    ));
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"Completed, Elapsed time: {sp.Elapsed}");

            using var session = store.OpenSession();

            Portfolio p = session.Load<Portfolio>("p3/2001/6/15");
        }
    }
}