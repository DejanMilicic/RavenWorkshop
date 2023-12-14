using System;
using System.Collections.Generic;
using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Commands.Batches;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Operations;

namespace Northwind.Features.Query
{
    public static class Query
    {
        public static void PropertyExists()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();

            // seed chats
            Chat chat1 = new Chat
            {
                Id = "Chats/1",
                Title = "chat 1",
                IsDeleted = false
            };
            session.Store(chat1);

            Chat chat2 = new Chat
            {
                Id = "Chats/2",
                Title = "chat 2",
                IsDeleted = true
            };
            session.Store(chat2);

            Chat chat3 = new Chat
            {
                Id = "Chats/3",
                Title = "chat 3",
                IsDeleted = false
            };
            session.Store(chat3);

            session.SaveChanges();

            // remove property IsDeleted from chat3
            session.Advanced.Defer(new PatchCommandData(
                id: chat3.Id,
                changeVector: null,
                patch: new PatchRequest
                {
                    Script = @"delete this.IsDeleted"
                },
                patchIfMissing: null));
            session.SaveChanges();

            // select chats that should not be deleted
            var notDeletedChatsWithError = session.Query<Chat>()
                .Where(c => !c.IsDeleted)
                .ToList();

            Console.WriteLine("Wrong list of not deleted chats:");
            foreach (Chat chat in notDeletedChatsWithError)
            {
                Console.WriteLine($"{chat.Id}");
            }

            // select chats that should not be deleted
            var notDeletedChats = session
                .Advanced
                .DocumentQuery<Chat>()
                .Not.WhereExists(chat => chat.IsDeleted) // property IsDeleted does not exist
                .OrElse()                                           // or
                .WhereEquals("IsDeleted", false)         // IsDeleted is false
                .ToList();

            Console.WriteLine();
            Console.WriteLine("Correct list of not deleted chats:");
            foreach (Chat chat in notDeletedChats)
            {
                Console.WriteLine($"{chat.Id}");
            }
        }

        public static void ByCollectionName()
        {
            using IDocumentStore store = new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "demo"
            }.Initialize();

            using var session = store.OpenSession();

            List<Employee> employees = session.Query<Employee>(collectionName: "Employee").ToList();
        }
    }

    public class Chat
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public bool IsDeleted { get; set; }
    }
}
