using System;
using System.IO;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Indexes.Analysis;
using Raven.Client.ServerWide.Operations.Analyzers;

namespace Northwind.Features.Analyzers.Snowball;

public static class DocumentStoreHolder
{
    private static readonly Lazy<IDocumentStore> LazyStore =
        new Lazy<IDocumentStore>(() =>
        {
            IDocumentStore store = new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "Products"
            };

            store.Initialize();

            store.Maintenance.Server.Send(new PutServerWideAnalyzersOperation(
                new AnalyzerDefinition
                {
                    Name = "SnowballAnalyzer",
                    Code = File.ReadAllText(Path.Combine(new[] { "..", "..", "..", "Features", "Analyzers", "Snowball", "SnowballAnalyzer.cs" }))
                }));

            IndexCreation.CreateIndexes(new AbstractIndexCreationTask[] { new Product_Search() }, store);

            return store;
        });

    public static IDocumentStore Store => LazyStore.Value;
}
