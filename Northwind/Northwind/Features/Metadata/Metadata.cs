using System;
using System.Linq;
using FluentAssertions;
using Northwind.Models.Entity;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Metadata
{
    public static class Metadata
    {
        public static void Create()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();
            var emp = session.Load<Employee>("employees/8-A");

            session.Advanced.GetMetadataFor(emp)["IsDeleted"] = "true";
            
            // todo: add example with json structure being stored to metadata

            session.SaveChanges();
        }

        public static void Read()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();
            var emp = session.Load<Employee>("employees/8-A");

            string isDeleted = (string)session.Advanced.GetMetadataFor(emp)["IsDeleted"];

            Console.WriteLine($"{emp.FirstName}: {isDeleted}");
        }

        public static void FetchJustMetadata()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            var metadata = session.Query<Employee>()
                .Where(x => x.Id == "employees/8-A")
                .Select(x => RavenQuery.Metadata(x)).SingleOrDefault()
                .As<IMetadataDictionary>();

            if (metadata.ContainsKey("IsDeleted"))
            {
                Console.WriteLine($"Entity contains metadata key IsDeleted: {metadata["IsDeleted"]}");
            }
            else
            {
                Console.WriteLine($"Entity does not contain metadata key IsDeleted");
            }
        }
    }
}
