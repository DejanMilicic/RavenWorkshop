using System;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace Northwind
{
    public static class DocumentStoreHolder
    {
        private static readonly Lazy<IDocumentStore> LazyStore =
            new Lazy<IDocumentStore>(() =>
            {
                IDocumentStore store = new DocumentStore
                {
                    Urls = new[] { "http://127.0.0.1:8080" },
                    Database = "demo"
                };

                store.Initialize();

                IndexCreation.CreateIndexes(typeof(Program).Assembly, store);

                return store;
            });

        public static IDocumentStore Store => LazyStore.Value;
    }
}
