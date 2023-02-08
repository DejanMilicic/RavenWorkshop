using System.Collections.Generic;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Spectre.Console;

namespace Northwind.Features.Indexes.DynamicFields.HeterogeneousDocuments
{
    public static class HeterogeneousDocumentsIndexing
    {
        private static IDocumentStore GetStore()
        {
            var store = (new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "demo"
            }).Initialize();

            IndexCreation.CreateIndexes(new AbstractIndexCreationTask[] { new Documents_ByAnyField() }, store);

            return store;
        }

        public static void Demo()
        {
            using var session = GetStore().OpenSession();

            List<dynamic> matchingDocuments = session
                .Advanced
                .DocumentQuery<object, Documents_ByAnyField>()
                .WhereEquals("Name", "Western")
                .OrElse()
                .WhereEquals("Name", "B's Beverages")
                .ToList();

            AnsiConsole.Markup($"\n[black on yellow]Results[/]\n\n");
            foreach (dynamic doc in matchingDocuments)
                AnsiConsole.WriteLine(doc.Id.ToString());
        }
    }
}
