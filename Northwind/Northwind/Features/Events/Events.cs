using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using Northwind.Models;

namespace Northwind.Features.Events
{
    public class Events
    {
        public void Create()
        {
            using var session = Dsh.Store.OpenSession();

            Employee emp = new Employee();
            emp.FirstName = "Marco";
            emp.LastName = "Polo";

            session.Store(emp);
            session.SaveChanges();
        }

        public void Delete()
        {
            using var session = Dsh.Store.OpenSession();
            var emp = session.Load<Employee>("employees/8-A");
            session.Delete(emp);
            session.SaveChanges();
        }

        public void Query()
        {
            using var session = Dsh.Store.OpenSession();

            var employees = session.Query<Employee>().ToList();

            Console.WriteLine(employees.Count);
        }

        public void Query3()
        {
            using var session = Dsh.Store.OpenSession();
            //session.Advanced.OnBeforeStore +=

            var employees = session.Query<Employee>().ToList();

            Console.WriteLine(employees.Count);
        }
    }

    public static class Dsh
    {
        private static readonly Lazy<IDocumentStore> LazyStore =
            new Lazy<IDocumentStore>(() =>
            {
                IDocumentStore store = new DocumentStore
                {
                    Urls = new[] { "http://127.0.0.1:8080" },
                    Database = "demo"
                };

                //store.OnBeforeConversionToDocument += (sender, eventArgs) =>
                //{
                //    if (eventArgs.Entity is Employee employee)
                //        employee.FirstName = employee.FirstName + "_2";
                //};

                //store.OnBeforeStore += (sender, eventArgs) =>
                //{
                //    if (eventArgs.Entity is Employee employee)
                //        if (employee.HiredAt == default)
                //            employee.HiredAt = DateTime.UtcNow;
                //};

                // Hands On 1 : Add the client ID of the user before storing the user
                // OnBeforeStore is executed immediately before session.Store(obj)
                //
                // var http = ctx.GetRequiredService<IHttpContextAccessor>();
                // var identity = http.HttpContext?.User.Identity;
                //
                store.OnBeforeStore += (sender, e) =>
                {
                    if (e.Session.GetChangeVectorFor(e.Entity) == null)
                        e.DocumentMetadata["Created-By"] = "[identity]";
                    e.DocumentMetadata["Modified-By"] = "[identity]";
                };

                // Hands On 2 : Prevent deletion of Employees with a specific name
                store.OnBeforeDelete += (sender, e) =>
                {
                    if (e.Entity is Employee)
                        throw new InvalidOperationException();
                };

                store.OnBeforeQuery += (sender, e) =>
                {
                    if (e.QueryCustomization is IDocumentQuery<Employee> qe)
                    {
                        qe.AndAlso().WhereEquals("Address.City", "London");
                    }
                };

                // app-level multitenancy

                // var http = ctx.GetRequiredService<IHttpContextAccessor>();
                // var identity = http.HttpContext?.User.Identity;

                store.OnBeforeQuery += (sender, e) =>
                {
                    // if (identity != null)
                    if (e.QueryCustomization is IDocumentQuery<MultitenantEntity> qe)
                    {
                        qe.AndAlso().Where(x => x.Tenant == "[identity.Name]");
                    }
                };

                // app-level soft delete implementation 
                store.OnBeforeQuery += (sender, e) =>
                {
                    if (e.QueryCustomization is IDocumentQuery<AuditedEntity> qe)
                    {
                        qe.AndAlso().Where(x => x.IsDeleted == false);
                    }
                };

                store.OnSessionCreated += (sender, e) =>
                {
                    e.Session.UseOptimisticConcurrency = true;
                };

                store.Initialize();

                IndexCreation.CreateIndexes(typeof(Program).Assembly, store);

                return store;
            });

        public static IDocumentStore Store => LazyStore.Value;
    }

    public static class Dsh2
    {
        public static void ShippedNotificationsDemo()
        {
            IDocumentStore store = new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "demoOrders"
            };

            // create command when order is created
            // without creating new commands on updates
            store.OnBeforeStore += (_, args) =>
            {
                if (args.Entity is Order o)
                {
                    if (o.ShippedAt is not null && args.DocumentMetadata.ContainsKey("ShippedNotification") == false)
                    {
                        args.DocumentMetadata["ShippedNotification"] = true;
                        args.Session.Store(new OrderShippedNotification(o));
                    }
                }
            };

            store.Initialize();

            using var session = store.OpenSession();

            Order order = new Order
            {
                ShippedAt = DateTime.Now,
                Company = "companies/1",
                Employee = "employees/1",
                Lines = new List<OrderLine>
                {
                    new OrderLine
                    {
                        ProductName = "apple",
                        Quantity = 5,
                        PricePerUnit = 1
                    }
                }
            };

            session.Store(order);
            session.SaveChanges();

            var o = session.Load<Order>(order.Id);
            o.ShippedAt = DateTime.Now.AddDays(7);
            session.SaveChanges();
        }

        public class OrderShippedNotification
        {
            public string Id { get; set; }

            public string OrderId { get; set; }

            public OrderShippedNotification(Order order)
            {
                this.OrderId = order.Id;
            }
        }
    }
}
