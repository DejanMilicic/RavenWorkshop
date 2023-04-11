using Northwind.Models.Entity;
using Raven.Client.Documents;
using System;

namespace Northwind.Features.DocumentSession;

public static class IdentityMap
{
    public static void Demo()
    {
        using var store = new DocumentStore { Urls = new[] { "http://127.0.0.1:8080" }, Database = "demo" }.Initialize();

        using var session = store.OpenSession();

        // this will go to the server, fetch JSON document employees/8-A
        // received JSON will be deserialized into strongly typed object "laura"
        // additionally, session will store document in the Identity map
        Employee laura = session.Load<Employee>("employees/8-A");

        // By default, when loading document by ID
        // session will first check inside of Identity Map
        // and if document with ID is found, it will be returned from it
        // without making a call to the server
        session.Load<Employee>("employees/8-A");

        // hence, one call to the database
        Console.WriteLine($"Total requests: {session.Advanced.NumberOfRequests}");
    }
}

