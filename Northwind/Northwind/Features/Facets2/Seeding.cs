using Northwind.Features.Facets2.Models;
using Raven.Client.Documents.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.Facets2
{
    internal static class Seeding
    {
        internal static void Do()
        {
            DatabaseStatistics? stats = FacetsStoreHolder.Store.Maintenance.Send(new GetStatisticsOperation());
            if (stats.CountOfDocuments > 0) return;

            var product1 = new Product
            {
                Id = "Products/Nike-shoes",
                Title = "Nike shoes",
                Brand = "Nike",
                Options = new List<ProductOption>
                {
                    new ProductOption
                    {
                        OptionId = 1,
                        Price = 15,
                        Available = false,
                        Attributes = new List<ProductOptionAttribute>
                        {
                            new ProductOptionAttribute { Name = "Size", Value = "9" },
                            new ProductOptionAttribute { Name = "Color", Value = "Red" }
                        }
                    },
                    new ProductOption
                    {
                        OptionId = 2,
                        Price = 15,
                        Available = true,
                        Attributes = new List<ProductOptionAttribute>
                        {
                            new ProductOptionAttribute { Name = "Size", Value = "10" },
                            new ProductOptionAttribute { Name = "Color", Value = "Red" }
                        }
                    },
                    new ProductOption
                    {
                        OptionId = 3,
                        Price = 15,
                        Available = false,
                        Attributes = new List<ProductOptionAttribute>
                        {
                            new ProductOptionAttribute { Name = "Size", Value = "11" },
                            new ProductOptionAttribute { Name = "Color", Value = "Red" }
                        }
                    },
                    new ProductOption
                    {
                        OptionId = 4,
                        Price = 17,
                        Available = false,
                        Attributes = new List<ProductOptionAttribute>
                        {
                            new ProductOptionAttribute { Name = "Size", Value = "9" },
                            new ProductOptionAttribute { Name = "Color", Value = "Black" }
                        }
                    },
                    new ProductOption
                    {
                        OptionId = 5,
                        Price = 17,
                        Available = true,
                        Attributes = new List<ProductOptionAttribute>
                        {
                            new ProductOptionAttribute { Name = "Size", Value = "10" },
                            new ProductOptionAttribute { Name = "Color", Value = "Black" }
                        }
                    },
                    new ProductOption
                    {
                        OptionId = 6,
                        Price = 17,
                        Available = false,
                        Attributes = new List<ProductOptionAttribute>
                        {
                            new ProductOptionAttribute { Name = "Size", Value = "11" },
                            new ProductOptionAttribute { Name = "Color", Value = "Black" }
                        }
                    }
                }
            };

            var product2 = new Product
            {
                Id = "Products/Adidas-shoes",
                Title = "Adidas shoes",
                Brand = "Adidas",
                Options = new List<ProductOption>
                {
                    new ProductOption
                    {
                        OptionId = 7,
                        Price = 12,
                        Available = true,
                        Attributes = new List<ProductOptionAttribute>
                        {
                            new ProductOptionAttribute { Name = "Size", Value = "9" },
                            new ProductOptionAttribute { Name = "Color", Value = "Red" }
                        }
                    },
                    new ProductOption
                    {
                        OptionId = 8,
                        Price = 12,
                        Available = false,
                        Attributes = new List<ProductOptionAttribute>
                        {
                            new ProductOptionAttribute { Name = "Size", Value = "11" },
                            new ProductOptionAttribute { Name = "Color", Value = "Red" }
                        }
                    },
                    new ProductOption
                    {
                        OptionId = 9,
                        Price = 12,
                        Available = true,
                        Attributes = new List<ProductOptionAttribute>
                        {
                            new ProductOptionAttribute { Name = "Size", Value = "9" },
                            new ProductOptionAttribute { Name = "Color", Value = "Yellow" }
                        }
                    },
                    new ProductOption
                    {
                        OptionId = 11,
                        Price = 15,
                        Available = true,
                        Attributes = new List<ProductOptionAttribute>
                        {
                            new ProductOptionAttribute { Name = "Size", Value = "10" },
                            new ProductOptionAttribute { Name = "Color", Value = "Yellow" }
                        }
                    },
                    new ProductOption
                    {
                        OptionId = 12,
                        Price = 15,
                        Available = false,
                        Attributes = new List<ProductOptionAttribute>
                        {
                            new ProductOptionAttribute { Name = "Size", Value = "11" },
                            new ProductOptionAttribute { Name = "Color", Value = "Yellow" }
                        }
                    },
                    new ProductOption
                    {
                        OptionId = 12,
                        Price = 18,
                        Available = true,
                        Attributes = new List<ProductOptionAttribute>
                        {
                            new ProductOptionAttribute { Name = "Size", Value = "12" },
                            new ProductOptionAttribute { Name = "Color", Value = "Yellow" }
                        }
                    }
                }
            };

            using var session = FacetsStoreHolder.Store.OpenSession();
            session.Store(product1);
            session.Store(product2);
            session.SaveChanges();
        }
    }
}
