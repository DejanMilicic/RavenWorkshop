using System;
using System.Collections.Generic;
using Northwind.Features.Identifiers;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Northwind.Features.DocumentSession
{
    public static class DocumentSession
    {
        public static void ExplicitDatabase()
        {
            using var session = DocumentStoreHolder.Store.OpenSession(database: "demo2");

            Employee laura = session.Load<Employee>("employees/8-A");
            
            // todo
            //laura.LastName = "Doe";
            //var changes = session.Advanced.WhatChanged();

            Console.WriteLine($"{laura.FirstName} {laura.LastName}");
        }

        public static void IdentityMap()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee laura = session.Load<Employee>("employees/8-A");
            
            laura = session.Load<Employee>("employees/8-A");
            
            Console.WriteLine($"Total requests: {session.Advanced.NumberOfRequests}");
        }

        public static void Evict()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee laura = session.Load<Employee>("employees/8-A");
            
            session.Advanced.Evict(laura); // stop tracking specified entity

            laura = session.Load<Employee>("employees/8-A");
            
            Console.WriteLine($"Total requests: {session.Advanced.NumberOfRequests}");
        }

        public static void SessionClear()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee laura = session.Load<Employee>("employees/8-A");
            Employee robert = session.Load<Employee>("employees/7-A");
            
            session.Advanced.Clear(); // stop tracking all entities

            laura = session.Load<Employee>("employees/8-A");
            robert = session.Load<Employee>("employees/7-A");
            
            Console.WriteLine($"Total requests: {session.Advanced.NumberOfRequests}");
        }

        public static void NoTracking()
        {
            using var session = DocumentStoreHolder.Store.OpenSession(new SessionOptions
            {
                NoTracking = true
            });
            
            Employee laura = session.Load<Employee>("employees/8-A");
            Employee robert = session.Load<Employee>("employees/7-A");
            
            laura = session.Load<Employee>("employees/8-A");
            robert = session.Load<Employee>("employees/7-A");
            
            Console.WriteLine($"Total requests: {session.Advanced.NumberOfRequests}");
        }

        public static void SelectiveTracking()
        {
            var store = (DocumentStore)DocumentStoreHolder.GetStore();

            store.Conventions.ShouldIgnoreEntityChanges =
                (session, entity, id) => (entity is Employee e) && (e.FirstName == "Laura");
            store.Initialize();

            using var session = store.OpenSession();
            
            Employee laura = session.Load<Employee>("employees/8-A"); // laura will not be tracked
            Employee robert = session.Load<Employee>("employees/7-A");

            laura.LastName += " CHANGED";
            robert.LastName += " CHANGED";

            session.SaveChanges();
        }

        public static void WhatChanged()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee laura = session.Load<Employee>("employees/8-A");
            laura.FirstName = Guid.NewGuid().ToString();
            laura.LastName = Guid.NewGuid().ToString();

            Employee newEmployee = new Employee();
            newEmployee.FirstName = "Marco";
            session.Store(newEmployee);

            IDictionary<string, DocumentsChanges[]> whatChanged = session.Advanced.WhatChanged();
            int countBeforeSave = whatChanged.Count;
            Console.WriteLine($"What changed before save changes: {countBeforeSave}");

            session.SaveChanges();

            int countAfterSave = session.Advanced.WhatChanged().Count;
            Console.WriteLine($"What changed after save changes: {countAfterSave}");
        }
    }
}
