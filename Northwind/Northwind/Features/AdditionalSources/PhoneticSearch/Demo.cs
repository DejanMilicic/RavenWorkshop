using System;
using System.Linq;
using Raven.Client.Documents;

namespace Northwind.Features.AdditionalSources.PhoneticSearch;

public static class Demo
{
    public static void Run()
    {
        using var session = DocumentStoreHolder.Store.OpenSession();

        string term = "Ashcraft";

        session.Query<Person, People_BySoundex>()
            .ProjectInto<People_BySoundex.Entry>()
            .Where(x => x.SoundexName == Soundex.Compute(term))
            .ToList()
            .ForEach(x => Console.WriteLine($"{x.Id} - {x.Name}")); 
    }
    
    public static void Seed()
    {
        using var session = DocumentStoreHolder.Store.OpenSession();

        session.Store(new Person {Name = "Rupert" });
        session.Store(new Person {Name = "Rubin" });
        session.Store(new Person {Name = "Ashcraft" });
        session.Store(new Person {Name = "Ashcroft" });

        session.SaveChanges();
    }
}
