using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using System.Collections.Generic;
using Northwind.Models.Entity;
using Spectre.Console;

namespace Northwind.Features.Indexes.DynamicFields.AllProperties;

public static class AllPropertiesIndexing
{
    private static IDocumentStore GetStore()
    {
        var store = (new DocumentStore
        {
            Urls = new[] { "http://127.0.0.1:8080" },
            Database = "Demo"
        }).Initialize();

        IndexCreation.CreateIndexes(new AbstractIndexCreationTask[] { new Products_ByAllAttributes() }, store);

        return store;
    }

    public static void Demo()
    {
        using var session = GetStore().OpenSession();

        List<Product> matchingProducts = 
            session.Advanced.DocumentQuery<Product, Products_ByAllAttributes>()
            .WhereEquals("Name", "Flotemysost")
            .OrElse()
            .WhereEquals("PricePerUnit", "13")
            .ToList();

        AnsiConsole.Markup($"\n[black on yellow]Products[/]\n\n");
        foreach (var prod in matchingProducts)
            AnsiConsole.WriteLine($"{prod.Id} - {prod.Name} - {prod.PricePerUnit}");
    }
}

