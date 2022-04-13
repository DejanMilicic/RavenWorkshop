using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Features.IndexingRelationships;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Polymorphism2
{
    public static class Seeding
    {
        public static IDocumentStore GetStore()
        {
            IDocumentStore store = new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "animals",
                //Conventions =
                //{
                //    FindCollectionName = type =>
                //    {
                //        if (typeof(Models.Animal).IsAssignableFrom(type))
                //            return "Animals";
                //        return DocumentConventions.DefaultGetCollectionName(type);
                //    }
                //}
            };

            store.Initialize();

            IndexCreation.CreateIndexes(new AbstractIndexCreationTask[]
            {
                new CatsParrots()
            }, store);

            return store;
        }

        public static void Seed()
        {
            var session = GetStore().OpenSession();

            Models.Cat cat = new Models.Cat
            {
                Name = "Persian",
                Breed = "Long fur"
            };
            session.Store(cat);

            Models.Parrot parrot = new Models.Parrot
            {
                Name = "Coco",
                Color = "Red"
            };
            session.Store(parrot);

            session.SaveChanges();
        }
    }
}
