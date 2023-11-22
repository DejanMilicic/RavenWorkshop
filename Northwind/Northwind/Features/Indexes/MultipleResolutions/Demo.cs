using System;
using System.Collections.Generic;
using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Spectre.Console;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace Northwind.Features.Indexes.MultipleResolutions;

public static class MultipleResolutionsIndexing
{
    private static IDocumentStore GetStore()
    {
        var store = (new DocumentStore
        {
            Urls = new[] { "http://127.0.0.1:8080" },
            Database = "Demo"
        }).Initialize();

        IndexCreation.CreateIndexes(new AbstractIndexCreationTask[] { new Orders_ByMultipleResolutions() }, store);

        return store;
    }

    public static void Demo()
    {
        using var session = GetStore().OpenSession();

        var results =
            session
                .Query<Orders_ByMultipleResolutions.Entry, Orders_ByMultipleResolutions>()
                .Where(x => 
                        x.Format == "yyyy-MM" 
                        && x.FormattedDate == "1997-05"
                        && x.Count > 1
                        )
                .OrderBy(x => x.FormattedDate)
                .ThenByDescending(x => x.Count)
                .ToList();

        AnsiConsole.Markup($"\n[black on yellow]Order Aggregations[/]\n\n");
        foreach (var r in results)
            AnsiConsole.WriteLine($"{r.FormattedDate} : {r.Company} - {r.Count}");
    }
}

