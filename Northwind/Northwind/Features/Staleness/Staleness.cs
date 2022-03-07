using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Staleness
{
    /// <summary>
    /// Demonstration of various staleness situations
    /// </summary>
    public static class Staleness
    {
        public static void NonStaleOperations()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Worker jane = new Worker { Id = "workers/1-A", Name = "jane" };
            session.Store(jane);

            Worker john = new Worker { Id = "workers/2-A", Name = "john" };
            session.Store(john);

            // save new entities to the database
            session.SaveChanges();
            session.Advanced.Clear();

            // operations that will always return non-stale results
            // from local node:

            // 1. load document by id
            Worker worker = session.Load<Worker>("workers/1-A");

            // 2. get all documents from collection
            List<Worker> workers = session.Query<Worker>().ToList();

            // 3. query by id
            workers = session.Query<Worker>()
                .Where(x => x.Id == "workers/1-A")
                .ToList();

            // 4. query by list of ids
            workers = session.Advanced.DocumentQuery<Worker>()
                .WhereIn(x => x.Id, new []{"workers/1-A", "workers/2-A"})
                .ToList();

            workers = session.Query<Worker>()
                .Where(x => x.Id.In("workers/1-A", "workers/2-A"))
                .ToList();

            // 5. query by id prefix
            workers = session.Query<Worker>()
                .Where(x => x.Id.StartsWith("workers/"))
                .ToList();
        }

        public static void StalenessPossible()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Worker jane = new Worker { Id = "workers/1-A", Name = "jane" };
            session.Store(jane);

            Worker john = new Worker { Id = "workers/2-A", Name = "john" };
            session.Store(john);

            // save new entities to the database
            session.SaveChanges();

            // on the local node, after documents are created or updated
            // all affected indexes will be updated
            // between moment of write and index update is completed, index is in stale state
            // if you query via index that is stale, you might get results which are outdated
            
            // 1. this query will trigger creation of auto index (if it already does not exist)
            //    and might return stale results
            List<Worker> workers = session.Query<Worker>()
                .Where(x => x.Name == "jane")
                .ToList();

            // 2. you can check if index that was used by query is stale or not
            workers = session.Query<Worker>()
                .Statistics(out QueryStatistics stats)
                .Where(x => x.Name == "jane")
                .ToList();
            bool isStale = stats.IsStale; // is it stale?
            string indexUsed = stats.IndexName; // and you can also find out which index was used

            // in most of the scenarios, you can allow query results that might be slightly out of date
            // however, sometimes you need exact and accurate results

            // 3. customize query to wait for non-stale results
            // if timespan is not specified, default value of 15s will be used
            workers = session.Query<Worker>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .Where(x => x.Name == "jane")
                .ToList();

            // 4. instead of customizing query to wait for non-stale results
            // you can instruct RavenDB to update all affected indexes in the same transaction
            session.Advanced.WaitForIndexesAfterSaveChanges();
            session.Store(new Worker { Id = "workers/3-A", Name = "marco" });
            session.SaveChanges(); // this call will not end until all affected indexes are updated

            // 5. or you can specify a list of indexes to wait on for and additional options
            session.Advanced.WaitForIndexesAfterSaveChanges(
                timeout: TimeSpan.FromSeconds(5),
                throwOnTimeout: false,
                indexes: new[] { "Auto/Workers/ByName" });
        }
    }

    public class Worker
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
