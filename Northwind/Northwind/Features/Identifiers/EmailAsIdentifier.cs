using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Northwind.Models.Entity;
using Raven.Client;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;

namespace Northwind.Features.Identifiers
{
    // todo : custom Id generation based on email as a suffix

    // todo : also
    // [JsonIgnore]
    // public string Id { get; set; }

    public class EmailEmployee
    {
        public string Email { get; set; }

        public string Name { get; set; }
    }

    public class EmailAsIdentifier
    {
        public void Do()
        {
            #region Customize Store

            IDocumentStore store = DocumentStoreHolder.GetStore();

            var defaultFindIdentityProperty = store.Conventions.FindIdentityProperty;

            store.Conventions.FindIdentityProperty = property =>
                typeof(EmailEmployee).IsAssignableFrom(property.DeclaringType)
                    ? property.Name == "Email"
                    : defaultFindIdentityProperty(property);

            store.Initialize();

            #endregion

            using (var session = store.OpenSession())
            {
                EmailEmployee emp = new EmailEmployee
                {
                    Email = "john@doe.com",
                    Name = "John"
                };

                session.Store(emp);
                var empId = session.Advanced.GetDocumentId(emp);
                Console.WriteLine($"id(emp) = {empId}");

                session.SaveChanges();
                
                session.Advanced.Evict(emp);
                EmailEmployee loadedEmp = session.Load<EmailEmployee>(empId);
                Console.WriteLine($"loadedEmp.Email = {loadedEmp.Email}");
            }
        }
    }
}
