using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Northwind.Features.AdditionalSources.AveragePrice;
using Northwind.Features.Attachments;
using Northwind.Features.BulkInsert;
using Northwind.Features.Caching;
using Northwind.Features.Changes;
using Northwind.Features.ChangeVector;
using Northwind.Features.Client;
using Northwind.Features.ClusterWideTransactions;
using Northwind.Features.Compression;
using Northwind.Features.Counters;
using Northwind.Features.Create;
using Northwind.Features.CRUD;
using Northwind.Features.Events;
using Northwind.Features.Expiration;
using Northwind.Features.Identifiers;
using Northwind.Features.Includes;
using Northwind.Features.Indexes;
using Northwind.Features.Lazy;
using Northwind.Features.Metadata;
using Northwind.Features.Operations;
using Northwind.Features.OptimisticConcurrency;
using Northwind.Features.Paging;
using Northwind.Features.Patching;
using Northwind.Features.Polymorphism;
using Northwind.Features.Projections;
using Northwind.Features.Refresh;
using Northwind.Features.Revisions;
using Northwind.Features.Search;
using Northwind.Features.DocumentSession;
using Northwind.Features.DynamicGrouping;
using Northwind.Features.Filter;
using Northwind.Features.Sorting;
using Northwind.Features.Spatial;
using Northwind.Features.StaleIndex;
using Northwind.Features.Staleness;
using Northwind.Features.Streaming;
using Northwind.Features.Subscriptions;
using Northwind.Features.Timeseries;
using Northwind.Models.Entity;
using Northwind.Models.ValueObject;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Northwind.Features.Facets;
using Northwind.Features.Facets2;
using Northwind.Features.Fanout;
using Northwind.Features.Records;
using Northwind.Features.RequestExecutor;
using Northwind.Features.SearchBasic;
using Northwind.Features.InnerJoin;
using Northwind.Features.BulkInsert.RawJson;
using Northwind.Features.Query;
using Northwind.Features.Customizations;
using Northwind.Features.Indexes.DynamicFields.AllProperties;
using Northwind.Features.Indexes.MultipleResolutions;
using Northwind.Features.Conventions;
using Northwind.Features.Indexes.Recursive;

namespace Northwind
{
    public class Res
    {
        public string ProductId { get; set; }

        public string CompanyId { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            DocumentStoreHolder.Store.OpenSession();

            return;

            #region temp

            //EmployeeSeeding.Seed();

            //MyProductSeed.Do();

            //new Lazy().ManyCallsOptimizedViaLazily();

            //using var store = new DocumentStore
            //{
            //    Urls = new string[] { "https://a.free.dejanmilicic.ravendb.cloud/" },
            //    Certificate = new X509Certificate2("free.dejanmilicic.client.certificate.pfx"),
            //    Database = "demo"
            //}.Initialize();

            //await IndexCreation.CreateIndexesAsync(typeof(Program).Assembly, store);

            #endregion

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

            //Console.WriteLine(session.Advanced.NumberOfRequests);

            #endregion

            #region Get orders by a company - v2

            //using var session = store.OpenSession();

            //var orders = session.Query<Order>()
            //    .Include(order => order.Employee)
            //    .Where(order => order.Company == "Companies/2-A")
            //    .OrderByDescending(order => order.OrderedAt)
            //    .Take(10)
            //    .ToList();

            //foreach (Order order in orders)
            //{
            //    var employee = session.Load<Employee>(order.Employee);
            //    Console.WriteLine(order.Id + "\t" + order.OrderedAt + "\t" + employee.FirstName);
            //}

            //Console.WriteLine(session.Advanced.NumberOfRequests);

            #endregion

            #region Get orders by a employee (excedding number of requests limit)

            //var sp = Stopwatch.StartNew();
            //using var session = store.OpenSession();
            ////session.Advanced.MaxNumberOfRequestsPerSession = 100;

            //var orders = session.Query<Order>()
            //    .Where(order => order.Employee == "Employees/1-A")
            //    .OrderByDescending(order => order.OrderedAt)
            //    .Take(25)
            //    .ToList();

            //foreach (Order order in orders)
            //{
            //    var c = session.Load<Company>(order.Company);
            //    Console.WriteLine(order.Id + "\t" + order.OrderedAt + "\t" + c.Name);
            //}

            //Console.WriteLine(session.Advanced.NumberOfRequests);
            //Console.WriteLine(sp.Elapsed);

            #endregion

            #region All Employees in London

            ////from Employees
            ////where Address.City = "London"

            //using var session = store.OpenSession();

            //var employees = session.Query<Employee>()
            //    .Where(x => x.Address.City == "London")
            //    .ToList();

            //foreach (Employee employee in employees)
            //{
            //    Console.WriteLine(employee.FirstName + " " + employee.LastName);
            //}

            #endregion

            #region All Companies with a contact named Karl

            ////from Companies
            ////where search(Contact.Name, "Karl")

            //using var session = store.OpenSession();

            //var companies = session.Query<Company>()
            //    .Search(x => x.Contact.Name, "Karl")
            //    .ToList();

            //foreach (Company company in companies)
            //{
            //    Console.WriteLine(company.Name);
            //}

            #endregion

            #region Search products that company purchased

            //string searchTerm = "*a*";
            //string company = "companies/5-a";

            //using var session = store.OpenSession();
            //var results = session.Query<Products_ByCompany.Entry, Products_ByCompany>()
            //    .Search(x => x.Query, searchTerm)
            //    .Where(x => x.Company == company)
            //    .ProjectInto<Products_ByCompany.Entry>()
            //    .Select(x => new Res
            //    {
            //        CompanyId = x.Company,
            //        ProductId = x.Product
            //    })
            //    .Include(x => x.ProductId)
            //    .Distinct()
            //    .ToList();

            //foreach (Res res in results)
            //{
            //    Product p = session.Load<Product>(res.ProductId);
            //    Console.WriteLine($"Company: {res.CompanyId}, Product: {p.Id} {p.Name}");
            //}

            #endregion
        }
    }
}
