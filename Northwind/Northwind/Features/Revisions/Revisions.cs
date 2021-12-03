using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Northwind.Models.Entity;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Revisions
{
    public class Revisions
    {
        public void ForceCreation()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee employee = new Employee
            {
                FirstName = "Rev",
                LastName = "Revision"
            };

            session.Store(employee);
            session.SaveChanges();

            session.Advanced.Revisions.ForceRevisionCreationFor(employee);
            session.SaveChanges();

            employee.FirstName = "Rev2";
            session.SaveChanges();
        }

        public void UndoRedoSim()
        {
            string employeeId;
            Employee employee;

            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                // create new employee
                employee = new Employee();
                employee.FirstName = "First";
                session.Store(employee);
                employeeId = employee.Id;
                session.SaveChanges();

                // turn revisions on
                session.Advanced.Revisions.ForceRevisionCreationFor(employee);
                session.SaveChanges();
            }

            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                employee = session.Load<Employee>(employeeId);
                employee.FirstName = "Second";
                session.SaveChanges();
            }

            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                employee = session.Load<Employee>(employeeId);
                employee.FirstName = "Third";
                session.SaveChanges();
            }

            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var revisions = session.Advanced.Revisions.GetFor<Employee>(employeeId);
                var undo = revisions.Skip(1).First();
                session.Store(undo);
                session.SaveChanges();
            }
        }

        public void GetRevisionsForEmployee()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var revisions = session.Advanced.Revisions.GetFor<Employee>("employees/8-A");

            foreach (Employee revision in revisions)
            {
                Console.WriteLine(revision);
            }
        }

        public void GetRevisionsForOrder()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            List<Order> orderRevisions = session.Advanced.Revisions.GetFor<Order>("orders/823-A");

            foreach (Order revision in orderRevisions)
            {
                Console.WriteLine(revision.Lines.Count);
            }
        }

        public void GetChangeVectorsForRevisions()
        {
            List<string> vectors = new List<string>();

            using var session = DocumentStoreHolder.Store.OpenSession();

            Order order = session.Load<Order>("orders/823-A");
            vectors.Add(session.Advanced.GetChangeVectorFor(order));

            List<Order> orderRevisions = session.Advanced.Revisions.GetFor<Order>("orders/823-A");

            foreach (Order revision in orderRevisions)
            {
                vectors.Add(session.Advanced.GetChangeVectorFor(revision));
            }
        }

        public void ExtractChanges()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            Order order = session.Load<Order>("orders/823-A");
            Order revision = session.Advanced.Revisions.GetFor<Order>("orders/823-A").Skip(2).First();

            JsonConvert.PopulateObject(JsonConvert.SerializeObject(revision), order,
                new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });

            var changes = session.Advanced.WhatChanged();
        }

        public void PrintRevisions()
        {
            string id = "employees/1-A";

            var revisions = this.GetRevisions<Employee>(id);

            Console.WriteLine($"Revisions for {id}");

            foreach (Employee revision in revisions)
            {
                Console.WriteLine($"{revision.FirstName}");
            }
        }

        public List<T> GetRevisions<T>(string id)
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                List<T> revisions = session.Advanced.Revisions.GetFor<T>(id);

                if (revisions.Count < 2) return new List<T>();

                List<T> revs = new List<T>();

                string lastModified = session.Advanced.GetMetadataFor(revisions.First())["@last-modified"].ToString();

                foreach (T revision in revisions.Skip(1))
                {
                    if (lastModified != session.Advanced.GetMetadataFor(revision)["@last-modified"].ToString())
                        revs.Add(revision);
                }

                revs.Reverse();

                return revs;

                Console.WriteLine($"Total requests: {session.Advanced.NumberOfRequests}");
            }
        }

        public List<IDictionary<string, object>> GetRevisionsMetadata(string id)
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                List<MetadataAsDictionary> revMetas = session.Advanced.Revisions.GetMetadataFor(id, 0, int.MaxValue);
                        
                if (revMetas.Count < 2) return new List<IDictionary<string, object>>();

                List<IDictionary<string, object>> revs = new List<IDictionary<string, object>>();

                string lastModified = revMetas.First()[Constants.Documents.Metadata.LastModified].ToString();

                foreach (var revision in revMetas.Skip(1))
                    if (lastModified != revision[Constants.Documents.Metadata.LastModified].ToString())
                        revs.Add(new Dictionary<string, object>
                        {
                            { Constants.Documents.Metadata.ChangeVector, revision[Constants.Documents.Metadata.ChangeVector] },
                            { Constants.Documents.Metadata.LastModified, revision[Constants.Documents.Metadata.LastModified] }
                        });

                revs.Reverse();
                
                return revs;
            }
        }

        public void Print()
        {
            string id = "employees/1-A";

            using var session = DocumentStoreHolder.Store.OpenSession();

            Employee employee = session.Load<Employee>(id);
            Console.WriteLine($"{employee.FirstName} - {session.Advanced.GetChangeVectorFor(employee)}");
            Console.WriteLine("--------------");

            List<Employee> orderRevisions = session.Advanced.Revisions.GetFor<Employee>(id);

            foreach (Employee revision in orderRevisions)
            {
                Console.WriteLine($"{revision.FirstName} - {session.Advanced.GetChangeVectorFor(revision)}" +
                                  $" - {session.Advanced.GetMetadataFor(revision)["@last-modified"]}");
            }
        }
    }
}
