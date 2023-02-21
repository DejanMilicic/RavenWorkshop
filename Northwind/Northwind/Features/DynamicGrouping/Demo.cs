using System.Collections.Generic;
using Northwind.Features.Indexes.DynamicFields.HeterogeneousDocuments;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Spectre.Console;

namespace Northwind.Features.DynamicGrouping
{
    public static class DynamicGroupingDemo
    {
        public static IDocumentStore GetStore()
        {
            var store = (new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "demo"
            }).Initialize();

            return store;
        }
    }
}
