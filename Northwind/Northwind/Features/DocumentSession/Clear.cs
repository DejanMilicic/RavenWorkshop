﻿using Northwind.Models.Entity;
using Raven.Client.Documents;
using System;

namespace Northwind.Features.DocumentSession;

public static class Clear
{
    public static void Demo()
    {
        using var store = new DocumentStore { Urls = new[] { "http://127.0.0.1:8080" }, Database = "demo" }.Initialize();

        using var session = store.OpenSession();

        Employee laura = session.Load<Employee>("employees/8-A");
        Employee robert = session.Load<Employee>("employees/7-A");

        // stop tracking all entities that were tracked so far
        // this is equivalent of opening new session - you get fresh new session
        session.Advanced.Clear();

        // at this point, identity map is completely empty
        // so loading ANY entity will be forced to go to the server
        session.Load<Employee>("employees/8-A");
        session.Load<Employee>("employees/7-A");

        // hence, 4 requests in total
        Console.WriteLine($"Total requests: {session.Advanced.NumberOfRequests}");
    }
}

