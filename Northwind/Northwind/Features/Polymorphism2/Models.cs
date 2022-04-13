using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.Polymorphism2
{
    public class Models
    {
        public abstract class Animal
        {
            public string Id { get; set; }

            public string Name { get; set; }
        }

        public class Cat : Animal
        {
            public string Breed { get; set; }
        }

        public class Parrot : Animal
        {
            public string Color { get; set; }
        }
    }
}
