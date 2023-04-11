using System;
using Northwind.Models.Entity;
using Raven.Client.Documents;

namespace Northwind.Features.Staleness;

public static class SavingChanges
{
    private static readonly string Server = "http://127.0.0.1:8080";
    private static readonly string Db = "demo";


    public static void BasicNotWaiting()
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
            throwOnTimeout: false);
        // with this modification, when timeout period expires
        // exception will not be thrown, so code execution will not be interrupted

        session.Advanced.WaitForIndexesAfterSaveChanges(
            timeout: TimeSpan.FromSeconds(5),
            throwOnTimeout: false,
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

    public static void BasicReplication()
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

        // default behavior is applied
        // SaveChanges() will wait for one of the nodes in the cluster
        // to confirm that changes have been saved
        // at the same time, rest of the nodes in the cluster still do not contain these changes
        // which are yet to be propagated to them
    }

    public static void SessionWaitForReplication()
    {
        using var store = new DocumentStore { Urls = new[] { Server }, Database = Db }.Initialize();

        using var session = store.OpenSession();

        session.Advanced.WaitForReplicationAfterSaveChanges(replicas: 2);
        // you can change default session behavior to request multiple replicas
        // to be confirmed before SaveChanges completes
        // In this case, changes will be saved to two nodes before SaveChanges is over

        session.Advanced.WaitForReplicationAfterSaveChanges(majority: true);
        // in this case, server will compute number of node which represent majority
        // e.g. out of 3 nodes, majority is 2, for cluster of 5 nodes, majority is 3
        // and will confirm replication to majority of nodes before SaveChanges() completes

        session.Advanced.WaitForReplicationAfterSaveChanges(
            timeout: TimeSpan.FromSeconds(1),
            majority: true);
        // it is also possible to set timeout which will force-complete SaveChanges()
        // disregarding of actual number of replicas

        session.Advanced.WaitForReplicationAfterSaveChanges(
            timeout: TimeSpan.FromSeconds(1),
            throwOnTimeout: false,
            majority: true);
        // here, we are preventing exception being thrown if timeout is reached
    }

    public static void StoreWaitForReplication()
    {
        using var store = new DocumentStore { Urls = new[] { Server }, Database = Db }.Initialize();

        store.OnSessionCreated += (sender, sessionCreatedEventArgs) =>
            sessionCreatedEventArgs.Session.WaitForReplicationAfterSaveChanges();

        // instead of altering just one session
        // we can instruct store to apply waiting for replicas
        // for all sessions that will be created
    }
}
