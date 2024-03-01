using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Spectre.Console;

namespace Northwind.Features.Indexes.DynamicFields.ProductAttributes
{
    public static class DynamicProductAttributes
    {
        private static IDocumentStore GetStore()
        {
            var store = (new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "ProductCatalog"
            }).Initialize();

            IndexCreation.CreateIndexes(new AbstractIndexCreationTask[] { new Products_ByAttribute() }, store);
            IndexCreation.CreateIndexes(new AbstractIndexCreationTask[] { new Products_ByAttribute_Typed() }, store);

            return store;
        }

        public static void Demo()
        {
            using var session = GetStore().OpenSession();
            
            Seed.Execute(session);
            session.SaveChanges();

            List<Product> matchingProducts = session
                .Advanced
                .DocumentQuery<Product, Products_ByAttribute>()
                // 'Size' is a dynamic-index-field that was indexed from the Attributes object
                .WhereEquals("Size", 39)
                .ToList();

            AnsiConsole.Markup($"\n[black on yellow]Products of size 39[/]\n\n");
            foreach (var prod in matchingProducts)
                AnsiConsole.WriteLine(prod.SKU);

            matchingProducts = session
                .Advanced
                .DocumentQuery<Product, Products_ByAttribute>()
                .WhereEquals("Waterproof", true)
                .ToList();

            AnsiConsole.Markup($"\n[black on yellow]Waterproof products[/]\n\n");
            foreach (var prod in matchingProducts)
                AnsiConsole.WriteLine(prod.SKU);

            matchingProducts = session
                .Query<Product, Products_ByAttribute_Typed>()
                .Where(x => (string)x.Attributes["Color"] == "red")
                .ToList();

            AnsiConsole.Markup($"\n[black on yellow]Red products[/]\n\n");
            foreach (var prod in matchingProducts)
                AnsiConsole.WriteLine(prod.SKU);
        }
    }
}

// Timezone causes index to crsh
// TimeZone WestEurope not found exception
// 

