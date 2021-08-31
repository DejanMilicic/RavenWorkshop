using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Features.Indexes;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Queries;

namespace Northwind.Features.Projections
{
    public class Companies_ByContact : AbstractIndexCreationTask<Company>
    {
        public class Entry
        {
            public string Name { get; set; }

            public string Phone { get; set; }
        }

        public Companies_ByContact()
        {
            Map = companies => companies
                .Select(x => new Entry
                {
                    Name = x.Contact.Name,
                    Phone = x.Phone
                });

            // Entry property names are the same, there is no need to store fields
            // StoreAllFields(FieldStorage.Yes); 
        }
    }
    public class Companies_ByContact2 : AbstractIndexCreationTask<Company>
    {
        public class Entry
        {
            public string ContactName { get; set; }

            public string ContactPhone { get; set; }
        }

        public Companies_ByContact2()
        {
            Map = companies => companies
                .Select(x => new Entry
                {
                    ContactName = x.Contact.Name,
                    ContactPhone = x.Phone
                });

            // Entry properties are different from field names, we need to store them explicitly
            // If we do not store them, ProjectInto will be populated by nulls
            // try commenting out following line and executing Example2
            StoreAllFields(FieldStorage.Yes);
        }
    }

    public class ProjectInto
    {
        public void Example1()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            List<Company> companies = session
                .Query<Company, Companies_ByContact>()
                .ToList();

            List<Companies_ByContact.Entry> contacts = session
                .Query<Company, Companies_ByContact>()
                .ProjectInto<Companies_ByContact.Entry>()
                .ToList();

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }

        public void Example2()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            List<Company> companies = session
                .Query<Company, Companies_ByContact2>()
                .ToList();

            List<Companies_ByContact2.Entry> contacts = session
                .Query<Company, Companies_ByContact2>()
                .ProjectInto<Companies_ByContact2.Entry>()
                .ToList();

            Console.WriteLine($"Total number of requests: {session.Advanced.NumberOfRequests}");
        }
    }
}
