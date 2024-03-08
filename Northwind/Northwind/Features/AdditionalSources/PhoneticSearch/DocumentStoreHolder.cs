using Raven.Client.Documents.Indexes;
using Raven.Client.Documents;
using System;

namespace Northwind.Features.AdditionalSources.PhoneticSearch;

public static class DocumentStoreHolder
{
    private static readonly Lazy<IDocumentStore> LazyStore =
        new Lazy<IDocumentStore>(() =>
        {
            var store = (new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "People"
            }).Initialize();

            IndexCreation.CreateIndexes(new AbstractIndexCreationTask[] { new People_BySoundex() }, store);

            return store;
        });

    public static IDocumentStore Store => LazyStore.Value;
}
