using System;
using System.Collections.Generic;
using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Northwind.Features.DocumentSession;

public static class NoTracking
{
    public static void StopTrackingEntity()
    {
        using var store = new DocumentStore { Urls = new[] { "http://127.0.0.1:8080" }, Database = "demo" }.Initialize();
        using var session = store.OpenSession();

        // document is loaded, stored in Identity Map and tracked for changes 
        Employee laura = session.Load<Employee>("employees/8-A");

        laura.LastName += " CHANGED";

        // laura is not tracked anymore
        // SaveChanges() will "ignore" this object
        // but, document will still be loaded in Identity Map
        // so, attempt to load it will go to the server
        session.Advanced.IgnoreChangesFor(laura);

        laura.FirstName += " CHANGED";

        // no changes are saved, since Laura is not tracked at this point
        session.SaveChanges();

        // this Load will be satisfied via Identity Map where Laura is still held
        session.Load<Employee>("employees/8-A");

        // so total number of requests sent will be 1
        Console.WriteLine($"Total requests: {session.Advanced.NumberOfRequests}");
    }

    public static void SessionNoTracking()
    {
        using var store = new DocumentStore { Urls = new[] { "http://127.0.0.1:8080" }, Database = "demo" }.Initialize();

        using var session = store.OpenSession(new SessionOptions
        {
            NoTracking = true
        });

        session.Load<Employee>("employees/8-A");
        session.Load<Employee>("employees/7-A");

        // since session is not tracking loaded entities
        // when we load them again, they will not be present in Identity Map
        // and two more round trips will be made to the server
        session.Load<Employee>("employees/8-A");
        session.Load<Employee>("employees/7-A");

        // so total number of requests sent will be 4
        Console.WriteLine($"Total requests: {session.Advanced.NumberOfRequests}");
    }

    public static void QueryNoTracking()
    {
        using var store = new DocumentStore { Urls = new[] { "http://127.0.0.1:8080" }, Database = "demo" }.Initialize();

        using var session = store.OpenSession();

        // todo : replicate this for Load

        List<Employee> employeesResults = session.Query<Employee>()
            // all entities returned by Query will not be tracked for changes
            .Customize(x => x.NoTracking())
            .Where(x => x.Address.City == "London")
            .ToList();

        // following modification will not be tracked for SaveChanges
        employeesResults.First().LastName += " from London";

        // modification we made will not be persisted
        session.SaveChanges();
    }
}
