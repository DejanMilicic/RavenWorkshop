using System;
using System.Linq;
using Raven.Client.Documents;

namespace Northwind.Features.Analyzers.Snowball;

public static class Demo
{
    public static void Run()
    {
        using var session = DocumentStoreHolder.Store.OpenSession();

        session.Query<Product, Product_Search>()
            .Search(x => x.Name, "Polos")
            .ToList()
            .ForEach(x => Console.WriteLine($"{x.Id} - {x.Name}"));
    }

    public static void Seed()
    {
        using var session = DocumentStoreHolder.Store.OpenSession();

        session.Store(new Product
        {
            Name = "RavenDB"
        });

        session.Store(new Product
        {
            Name = "Polo"
        });

        session.Store(new Product
        {
            Name = "Polos"
        });

        session.SaveChanges();
    }
}
