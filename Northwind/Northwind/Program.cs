using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Northwind
{
    class Program
    {
        static void Main(string[] args)
        {
            using var store = new DocumentStore
            {
                Urls = new string[] { "https://a.d2.development.run/" },
                Certificate = new X509Certificate2("admin.client.certificate.d2.pfx"),
                Database = "demo"
            }.Initialize();

            #region Read entity from the database

            //using var session = store.OpenSession();
            //Employee employee = session.Load<Employee>("employees/8-A");
            //Console.WriteLine(employee.FirstName);

            #endregion

            #region Modify entity and save it to the database

            //using var session = store.OpenSession();
            //Employee employee = session.Load<Employee>("employees/8-A");
            //employee.LastName = employee.LastName.ToUpper();
            //session.SaveChanges();

            #endregion

            #region Session is tracking changes

            //using var session = store.OpenSession();
            //Employee emp = session.Load<Employee>("employees/8-A");
            //emp.FirstName = "Jane";
            //emp.LastName = "Doe";

            //IDictionary<string, DocumentsChanges[]> changes = session.Advanced.WhatChanged();
            //DocumentsChanges[] employeeChanges = changes["employees/8-A"];
            //DocumentsChanges change1 = employeeChanges[0];
            //DocumentsChanges change2 = employeeChanges[1];

            #endregion

            #region Counters

            //using var session = store.OpenSession();
            //var counters = session.CountersFor("products/74-A");

            //string twoStars = counters.Get("⭐⭐").ToString();
            //Console.WriteLine(twoStars);

            //counters.Increment("⭐⭐");
            //counters.Increment("⭐⭐⭐", -1);
            //session.SaveChanges();

            #endregion

            #region Revisions

            //using var session = store.OpenSession();
            //List<Order> orders = session.Advanced.Revisions.GetFor<Order>("orders/823-A");

            #endregion
        }
    }
}
