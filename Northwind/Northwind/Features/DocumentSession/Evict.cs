using Northwind.Models.Entity;
using Raven.Client.Documents;
using System;

namespace Northwind.Features.DocumentSession;

public static class Evict
{
    public static void Demo()
    {
        using var store = new DocumentStore { Urls = new[] { "http://127.0.0.1:8080" }, Database = "demo" }.Initialize();

        using var session = store.OpenSession();

        Employee laura = session.Load<Employee>("employees/8-A");
        session.Advanced.Evict(laura); // stop tracking specified entity
        
        // next time Load is called for the same ID
        // Identity Map does not contain entry for this ID
        // and document will be loaded one more time
        laura = session.Load<Employee>("employees/8-A");
        
        // hence, two requests to the server
        Console.WriteLine($"Total requests: {session.Advanced.NumberOfRequests}");
    }
}

