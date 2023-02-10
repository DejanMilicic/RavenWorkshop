using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Indexes.IndexingAttachments
{
    public static class Library
    {
        private static IDocumentStore GetStore()
        {
            var store = (new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "Library"
            }).Initialize();

            IndexCreation.CreateIndexes(new AbstractIndexCreationTask[] { new Library_Search() }, store);

            return store;
        }

        public static void Seed()
        {
            using var session = GetStore().OpenSession();

            string rootFolder = Path.Combine("Features", "Indexes", "IndexingAttachments");

            #region Romeo and Juliet

            var romeoJuliet = new Book()
            {
                Id = "Books/RomeoAndJuliet",
                Title = "Romeo and Juliet"
            };
            session.Store(romeoJuliet);

            string filename = "RomeoAndJuliet.txt";
            using FileStream romeoJulietteFullText = File.Open(Path.Combine(rootFolder, filename), FileMode.Open);
            session.Advanced.Attachments.Store(romeoJuliet.Id, filename, romeoJulietteFullText, "text/plain");

            #endregion

            #region Dracula

            var dracula = new Book()
            {
                Id = "Books/Dracula",
                Title = "Dracula"
            };
            session.Store(dracula);

            filename = "Dracula.zip";
            using FileStream draculaFullText = File.Open(Path.Combine(rootFolder, filename), FileMode.Open);
            session.Advanced.Attachments.Store(dracula.Id, filename, draculaFullText, "application/zip");

            #endregion

            session.SaveChanges();
        }

        public static void Search()
        {
            using var session = GetStore().OpenSession();

            string term = "love";

            List<Book> books = session.Query<Library_Search.Entry, Library_Search>()
                .Search(x => x.Content, term)
                .OfType<Book>()
                .ToList();

            Console.WriteLine($"Search results for: {term}");
            Console.WriteLine("===");
            foreach (Book book in books)
            {
                Console.WriteLine($"{book.Id} : {book.Title}");
            }
        }
    }
}
