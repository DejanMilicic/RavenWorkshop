using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Polymorphism2
{
    public class Animals : AbstractMultiMapIndexCreationTask<Animals.Entry>
    {
        public class Entry
        {
            public string Name { get; set; }

            public bool Flies { get; set; }

            public bool Furry { get; set; }

            public string Characteristic { get; set; }
        }

        public Animals()
        {
            AddMap<Models.Cat>(
                cats => from cat in cats
                    select new Entry
                    {
                        Name = cat.Name,
                        Flies = false,
                        Furry = true,
                        Characteristic = cat.Breed
                    }
            );

            AddMap<Models.Parrot>(
                parrots => from parrot in parrots
                    select new Entry
                    {
                        Name = parrot.Name,
                        Flies = true,
                        Furry = false,
                        Characteristic = parrot.Color
                    }
            );
        }
    }
}
