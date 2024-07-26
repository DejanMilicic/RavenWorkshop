using System;
using System.Collections.Generic;
using Raven.Client.Documents;
using Raven.Client.Exceptions;

namespace Northwind.Features.Patching.Upsert
{
    public static class UpsertPatchDemo
    {
        private static IDocumentStore GertStore()
        {
            return (new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "Clinic"
            }).Initialize();
        }

        /// When you are updating a document
        /// that has already been modified since you fetched it
        /// default strategy is "last write wins"
        /// meaning RavenDB will let you override changes created in the meantime
        public static void LastWriteWins()
        {
            var store = GertStore();

            using (var session = store.OpenSession())
            {
                Appointment app = new Appointment
                {
                    Id = "Appointment/1",
                    Doctor = "Udo Brinkmann",
                    Time = new DateTime(2023, 1, 1),
                    Patients = new List<Patient>()
                };

                session.Store(app);
                session.SaveChanges();
            }

            using (var session1 = store.OpenSession())
            {
                Appointment app1 = session1.Load<Appointment>("Appointment/1");
                app1.Patients.Add(new Patient { Name = "Alice" });

                using (var session2 = store.OpenSession())
                {
                    Appointment app2 = session2.Load<Appointment>("Appointment/1");
                    app2.Patients.Add(new Patient { Name = "Bob" });
                    session2.SaveChanges(); // this will execute first, saving Bob as a patient
                }

                session1.SaveChanges(); // this will execute second, overriding Bob with Alice

                // final outcome of this is just a single patient saved - Alice
            }
        }

        /// One possible way of avoiding this is by usage of Optimistic Concurrency
        /// RavenDB will use change vector to detect if you are updating same version of document
        /// as one currently in the database
        /// If you are attempting to overwrite newer version, ConcurrencyException will be thrown
        public static void OptimisticConcurrency()
        {
            var store = GertStore();

            using (var session = store.OpenSession())
            {
                Appointment app = new Appointment
                {
                    Id = "Appointment/1",
                    Doctor = "Udo Brinkmann",
                    Time = new DateTime(2023, 1, 1),
                    Patients = new List<Patient>()
                };

                session.Store(app);
                session.SaveChanges();
            }

            using (var session1 = store.OpenSession())
            {
                session1.Advanced.UseOptimisticConcurrency = true;
                
                Appointment app1 = session1.Load<Appointment>("Appointment/1");
                app1.Patients.Add(new Patient { Name = "Alice" });

                using (var session2 = store.OpenSession())
                {
                    Appointment app2 = session2.Load<Appointment>("Appointment/1");
                    app2.Patients.Add(new Patient { Name = "Bob" });
                    session2.SaveChanges(); // this will execute first, saving Bob as a patient
                }

                try
                {
                    // this will execute second, attempting to override Bob with Alice
                    session1.SaveChanges(); 
                }
                catch (ConcurrencyException ex)
                {
                    // but exception will be thrown, since session1 is using optimistic concurrency
                    // so we should handle it in a civilized manner

                    session1.Advanced.Evict("Appointment/1");
                    Appointment appointment = session1.Load<Appointment>("Appointment/1");
                    //var changes0 = session1.Advanced.WhatChanged();
                    appointment.Patients.Add(new Patient { Name = "Alice" });
                    //var changes1 = session1.Advanced.WhatChanged();
                    session1.SaveChanges();
                }

                // final outcome of this is both patients saved - [ Bob, Alice ]
            }
        }

        public static void DemoLastWriteWins()
        {
            Appointment app1 = new Appointment
            {
                Id = "Appointment/1",
                Doctor = "Udo Brinkmann",
                Time = new DateTime(2023, 1, 1),
                Patients = new List<Patient>()
            };

            Appointment app2 = new Appointment
            {
                Id = "Appointment/1",
                Doctor = "Fritz Lang",
                Time = new DateTime(2023, 1, 1),
                Patients = new List<Patient>()
            };

            var store = GertStore();

            using (var session = store.OpenSession())
            {
                session.Store(app1);
                session.SaveChanges();
            }

            using (var session = store.OpenSession())
            {
                session.Store(app2);
                session.SaveChanges();
            }

            // one more example of last write wins
            // final outcome of this is just a single appointment saved - app2
            // app1 is overwritten by app2, since they have the same ID

            // hence, with this approach, if document does not exist, it will be created
            // and if it exists, it will be overwritten/updated
        }

        public static void DemoPreventOverriding()
        {
            Appointment app1 = new Appointment
            {
                Id = "Appointment/1",
                Doctor = "Udo Brinkmann",
                Time = new DateTime(2023, 1, 1),
                Patients = new List<Patient>()
            };

            Appointment app2 = new Appointment
            {
                Id = "Appointment/1",
                Doctor = "Fritz Lang",
                Time = new DateTime(2023, 1, 1),
                Patients = new List<Patient>()
            };

            var store = GertStore();

            using (var session = store.OpenSession())
            {
                session.Store(app1);
                session.SaveChanges();
            }

            try
            {
                using (var session = store.OpenSession())
                {
                    session.Store(app2, changeVector: "", id: "Appointment/1");
                    session.SaveChanges();
                }
            }
            catch (ConcurrencyException _)
            {
                Console.WriteLine("Document was not saved, because it would override existing document");
            }

            // final outcome of this is just a single appointment saved - app1
            // app2 is not saved, because it would override app1
            // passing empty string as a change vector ensures document will be saved
            // only if it does not exist yet
        }
    }
}
