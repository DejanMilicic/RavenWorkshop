using System;
using Raven.Client.Documents;

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
                    Time = new DateTime(2023, 1, 1)
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
    }
}
