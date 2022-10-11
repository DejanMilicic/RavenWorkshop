using System;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.IndexingRelationships.Graph
{
    internal static class GraphStoreHolder
    {
        private static IDocumentStore GetStore()
        {
            return new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "graph"
            };
        }

        private static readonly Lazy<IDocumentStore> LazyStore =
            new Lazy<IDocumentStore>(() =>
            {
                IDocumentStore store = GetStore();

                store.Initialize();

                new Numbers_ByHierarchy().Execute(store);
                new Graph_ByDistance().Execute(store);

                return store;
            });

        public static IDocumentStore Store => LazyStore.Value;
    }
}
