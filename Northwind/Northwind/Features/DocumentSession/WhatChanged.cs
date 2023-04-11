using Northwind.Models.Entity;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using System;
using Raven.Client.Documents;

namespace Northwind.Features.DocumentSession;

public static class WhatChanged
{
    public static void Demo()
    {
        using var store = new DocumentStore { Urls = new[] { "http://127.0.0.1:8080" }, Database = "demo" }.Initialize();
        using var session = store.OpenSession();

        Employee laura = session.Load<Employee>("employees/8-A");
        laura.FirstName = Guid.NewGuid().ToString();
        laura.LastName = Guid.NewGuid().ToString();

        session.Store(new Employee
        {
            FirstName = "Marco"
        });

        IDictionary<string, DocumentsChanges[]> whatChanged = session.Advanced.WhatChanged();
        int countBeforeSave = whatChanged.Count;
        Console.WriteLine($"What changed before save changes: {countBeforeSave}");

        session.SaveChanges();

        int countAfterSave = session.Advanced.WhatChanged().Count;
        Console.WriteLine($"What changed after save changes: {countAfterSave}");
    }
}

