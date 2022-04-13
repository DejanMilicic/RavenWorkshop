using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Polymorphism2
{
    public class Animals : AbstractMultiMapIndexCreationTask<CatsParrots.Entry>
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
            // cats
            AddMap<Models.Animal>(
                animals => from animal in animals
                           where MetadataFor(animal)
                               .Value<string>("Raven-Clr-Type")
                               .Contains("Models+Cat")
                           select new Entry
                           {
                               Name = animal.Name,
                               Flies = false,
                               Furry = true,
                               Characteristic = ((Models.Cat)animal).Breed
                           }
            );

            // parrots
            AddMap<Models.Animal>(
                animals => from animal in animals
                           where MetadataFor(animal)
                               .Value<string>("Raven-Clr-Type")
                               .Contains("Models+Parrot")
                           select new Entry
                           {
                               Name = animal.Name,
                               Flies = true,
                               Furry = false,
                               Characteristic = ((Models.Parrot)animal).Color
                           }
            );
        }
    }
}
