using System;
using Northwind.Models.Entity;
using Raven.Client.Documents;

namespace Northwind.Features.Staleness;

public static class SavingChanges
{
    private static readonly string Server = "http://127.0.0.1:8080";
    private static readonly string Db = "demo";


    public static void Basic()
    {
        using var store = new DocumentStore { Urls = new[] { Server }, Database = Db }.Initialize();

        using var session = store.OpenSession();

        Employee employee = new Employee
        {
            FirstName = "Martha",
            LastName = "Smith"
        };

        session.Store(employee);

        session.SaveChanges();

        // at this point, document have been saved to the database
        // all affected indexes will be moved to "stale" state
        // and will start processing newly created document
    }

    public static void SessionWaitForIndexes()
    {
        // we can alter default behavior by instructing session
        // to wait for indexes to be updated after SaveChanges() is executed

        using var store = new DocumentStore { Urls = new[] { Server }, Database = Db }.Initialize();

        using var session = store.OpenSession();

        session.Advanced.WaitForIndexesAfterSaveChanges();
        // with this modification, every time SaveChanges() is called
        // it will not be completed until all affected indexes are updated
        // first, changed collections will be determined
        // after that, indexes operating over those collections will be waited for

        session.Advanced.WaitForIndexesAfterSaveChanges(
            timeout: TimeSpan.FromSeconds(5));
        // but, for how long are we willing to wait?
        // here, we will be waiting up to 5 seconds for SaveChanges() to complete
        // if after 5 seconds, affected indexes are still stale, SaveChanges() will complete

        session.Advanced.WaitForIndexesAfterSaveChanges(
            timeout: TimeSpan.FromSeconds(5),
            throwOnTimeout: true);
        // with this modification, when timeout period expires
        // exception will be thrown, interrupting code execution

        session.Advanced.WaitForIndexesAfterSaveChanges(
            timeout: TimeSpan.FromSeconds(5),
            throwOnTimeout: true,
            indexes: new[] { "index/1" });
        // instead of waiting for all indexes to be updated
        // we can wait just for those which are important to us
        // this way we are minimizing waiting time
    }

    public static void StoreWaitForIndexes()
    {
        using var store = new DocumentStore { Urls = new[] { Server }, Database = Db }.Initialize();

        store.OnSessionCreated += (sender, sessionCreatedEventArgs) =>
            sessionCreatedEventArgs.Session.WaitForIndexesAfterSaveChanges();

        // instead of altering just one session
        // we can instruct store to apply waiting for indexes
        // for all sessions that will be created
    }
}
