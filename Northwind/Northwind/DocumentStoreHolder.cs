using System;
using System.Security.Cryptography.X509Certificates;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace Northwind
{
    public static class DocumentStoreHolder
    {
        public static IDocumentStore GetStore()
        {
            return new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "bobit"
                //Certificate = new X509Certificate2(@"...\cert.pfx"),
            };
        }

        private static readonly Lazy<IDocumentStore> LazyStore =
            new Lazy<IDocumentStore>(() =>
            {
                IDocumentStore store = GetStore();

                store.Initialize();

                IndexCreation.CreateIndexes(typeof(Program).Assembly, store);

                return store;
            });

        public static IDocumentStore Store => LazyStore.Value;
    }
}
