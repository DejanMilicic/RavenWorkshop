using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Indexes.Recursive;

public static class RecursiveIndexing
{
    private static IDocumentStore GetStore()
    {
        var store = (new DocumentStore
        {
            Urls = new[] { "http://127.0.0.1:8080" },
            Database = "Demo"
        }).Initialize();

        IndexCreation.CreateIndexes(new AbstractIndexCreationTask[] { new Parts_BySubparts() }, store);

        return store;
    }

    public static void Seed()
    {
        using var session = GetStore().OpenSession();

        session.Store(new Part
        {
            Id = "parts/1",
            SubParts = new string[] { "parts/2", "parts/3" }
        });

        session.Store(new Part
        {
            Id = "parts/2",
            SubParts = new string[] { "parts/2.1", "parts/2.2" }
        });

        session.Store(new Part
        {
            Id = "parts/3",
            SubParts = new string[] {}
        });

        session.Store(new Part
        {
            Id = "parts/2.1",
            SubParts = new string[] { }
        });

        session.Store(new Part
        {
            Id = "parts/2.2",
            SubParts = new string[] { "parts/2.2.1" }
        });

        session.Store(new Part
        {
            Id = "parts/2.2.1",
            SubParts = new string[] { }
        });

        session.SaveChanges();
    }

    public static void Query()
    {
        using var session = GetStore().OpenSession();

        var subparts = session
            .Query<Parts_BySubparts.Entry, Parts_BySubparts>()
            .Where(x => x.Id == "parts/1")
            .Select(x => x.Subparts)
            .SingleOrDefault();

        Console.WriteLine($"Subparts of parts/1:");

        foreach (var subpart in subparts)
        {
            Console.WriteLine(subpart);
        }
    }
}

