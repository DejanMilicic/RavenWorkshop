using System;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Northwind.Features.DocumentSession;

public static class NoTracking
{
    public static void Demo()
    {
        using var store = new DocumentStore { Urls = new[] { "http://127.0.0.1:8080" }, Database = "demo" }.Initialize();

        using var session = DocumentStoreHolder.Store.OpenSession(new SessionOptions
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
}
