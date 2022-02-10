using System;
using System.Collections.Generic;
using Northwind.Models.Entity;
using Raven.Client.Documents.Session;

namespace Northwind.Features.DocumentSession
{
    public static class DocumentSession
    {
        public static void Evict()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee laura = session.Load<Employee>("employees/8-A");
            
            session.Advanced.Evict(laura);

            laura = session.Load<Employee>("employees/8-A");
            
            Console.WriteLine($"Total requests: {session.Advanced.NumberOfRequests}");
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
