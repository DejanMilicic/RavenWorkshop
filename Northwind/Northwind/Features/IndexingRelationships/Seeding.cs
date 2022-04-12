using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.IndexingRelationships
{
    public static class Seeding
    {
        public static IDocumentStore GetStore()
        {
            IDocumentStore store = new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "media"
            };

            store.Initialize();

            IndexCreation.CreateIndexes(new AbstractIndexCreationTask[]
            {
                new Users_ByContent(),
                new Users_ByContent2()
            }, store);

            return store;
        }

        public static void Seed()
        {
            var session = GetStore().OpenSession();

            Models.User ana = new Models.User { Name = "Ana" };
            session.Store(ana);

            Models.User bob = new Models.User { Name = "Bob" };
            session.Store(bob);

            Models.BlogPost postBasic = new Models.BlogPost
            {
                Title = "Basic tutorial",
                ReadingTime = 11
            };
            session.Store(postBasic);

            Models.BlogPost postAdvanced = new Models.BlogPost
            {
                Title = "Advanced tutorial",
                ReadingTime = 25
            };
            session.Store(postAdvanced);

            Models.Video video = new Models.Video
            {
                Title = "Video tutorial",
                WatchingTime = 45
            };
            session.Store(video);

            Models.UserActivity ana1 = new Models.UserActivity
            {
                User = ana.Id,
                Content = postBasic.Id
            };
            session.Store(ana1);

            Models.UserActivity ana2 = new Models.UserActivity
            {
                User = ana.Id,
                Content = postAdvanced.Id
            };
            session.Store(ana2);

            Models.UserActivity bob1 = new Models.UserActivity
            {
                User = bob.Id,
                Content = postAdvanced.Id
            };
            session.Store(bob1);

            Models.UserActivity bob2 = new Models.UserActivity
            {
                User = bob.Id,
                Content = video.Id
            };
            session.Store(bob2);

            session.SaveChanges();
        }
    }
}
