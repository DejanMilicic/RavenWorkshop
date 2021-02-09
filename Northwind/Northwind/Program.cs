using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
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

            #region Filter employees by first name

            //using var session = store.OpenSession();
            //var employees = session.Query<Employee>()
            //    .Where(x => x.FirstName == "Nancy")
            //    .ToList();

            //foreach (Employee employee in employees)
            //{
            //    Console.WriteLine(employee.LastName);
            //}

            #endregion

            #region Filter employees by first name - get query

            //using var session = store.OpenSession();
            //var employees = session.Query<Employee>()
            //    .Where(x => x.FirstName == "Nancy");

            //Console.WriteLine(employees);

            #endregion

            #region Orders by Employee #1

            //using var session = store.OpenSession();
            //var employeeIds = Queryable.Select(session.Query<Employee>()
            //        .Where(x => x.FirstName == "Nancy"), x => x.Id)
            //        .ToList();

            //var orders = session.Query<Order>()
            //    .Where(x => x.Employee.In(employeeIds));

            //Console.WriteLine(orders.ToString());

            //foreach (Order order in orders)
            //{
            //    Console.WriteLine(order.Id);
            //}

            #endregion

            #region Get orders by a company - v1

            //using var session = store.OpenSession();

            //var orders = session.Query<Order>()
            //    .Where(order => order.Company == "Companies/2-A")
            //    .OrderByDescending(order => order.OrderedAt)
            //    .Take(10)
            //    .ToList();

            //foreach (Order order in orders)
            //{
            //    var employee = session.Load<Employee>(order.Employee);
            //    Console.WriteLine(order.Id + "\t" + order.OrderedAt + "\t" + employee.FirstName);
            //}

            //int requests = session.Advanced.NumberOfRequests;

            #endregion
        }
    }
}
