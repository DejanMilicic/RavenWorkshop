using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.AdditionalSources.PhoneticSearch;

public class People_BySoundex : AbstractIndexCreationTask<Person, People_BySoundex.Entry>
{
    public class Entry
    {
        public string Id { get; set; }

        public string SoundexName { get; set; }

        public string Name { get; set; }
    }

    public People_BySoundex()
    {
        Map = people => 
                from person in people
                select new Entry
                {
                    SoundexName = Soundex.Compute(person.Name),
                    Name = person.Name
                };

        AdditionalSources = new Dictionary<string, string>
        {
            ["Soundex.cs"] =
                File.ReadAllText(Path.Combine(new[]
                    { AppContext.BaseDirectory, "..", "..", "..", "Features", "AdditionalSources", "PhoneticSearch", "Soundex.cs" }))
        };
    }
}

