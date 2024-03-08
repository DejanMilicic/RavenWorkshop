using System;
using System.IO;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Indexes.Analysis;
using Raven.Client.ServerWide.Operations.Analyzers;

namespace Northwind.Features.Analyzers.Multilang;

public static class DocumentStoreHolder
{
    private static readonly Lazy<IDocumentStore> LazyStore =
        new(() =>
        {
            IDocumentStore store = new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "Articles"
            };

            store.Initialize();

            // https://github.com/ravendb/lucenenet/tree/ravendb/v5.2/src/contrib/Analyzers/Fr
            store.Maintenance.Server.Send(new PutServerWideAnalyzersOperation(
                new AnalyzerDefinition
                {
                    Name = "FrenchAnalyzer",
                    Code = File.ReadAllText(Path.Combine(new []{"..", "..", "..", "Features", "Analyzers", "Multilang", "FrenchAnalyzer.cs" }))
                }));

            IndexCreation.CreateIndexes(new AbstractIndexCreationTask[] { new Articles_Search() }, store);

            return store;
        });

    public static IDocumentStore Store => LazyStore.Value;
}
