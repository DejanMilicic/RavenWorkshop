using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents;

namespace Northwind.Features.Analyzers.Multilang;

public static class Demo
{
    public static void Run()
    {
        var session = DocumentStoreHolder.Store.OpenSession();

        string term = "car";

        foreach (string lang in new List<string> { "fr", "en" })
            DisplayResults(lang, term,
                session.Query<Articles_Search.Entry, Articles_Search>()
                    .Search(x => x.Text[lang], term)
                    .ProjectInto<Article>()
                    .ToList());
    }

    public static void DisplayResults(string lang, string term, List<Article> articles)
    {
        Console.WriteLine();
        Console.WriteLine($"Searching '{lang}' content for term : {term}");

        if (articles.Count == 0)
            Console.WriteLine("\tNo results");
        foreach (Article article in articles)
            Console.WriteLine($"\t{article.Id}");
    }

    public static void Seed()
    {
        var session = DocumentStoreHolder.Store.OpenSession();

        session.Store(new Article
        {
            Text = "This is a text about car",
            Language = "en"
        });

        session.Store(new Article
        {
            Text = "Je n'ai pas faim car j'ai pris le petit déjeuner",
            Language = "fr"
        });

        session.Store(new Article
        {
            Text = "The quick brown fox jumps over the lazy dog or a car",
            Language = ""
        });

        session.SaveChanges();
    }
}
