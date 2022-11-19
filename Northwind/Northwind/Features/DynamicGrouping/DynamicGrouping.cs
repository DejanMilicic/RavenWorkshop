using System;
using System.Dynamic;
using Spectre.Console;
using System.Linq;

namespace Northwind.Features.DynamicGrouping
{
    public static class DynamicGrouping
    {
        private class Sneaker
        {
            public string Id { get; set; }

            public string Company { get; set; }

            public string Brand { get; set; }

            public string Country { get; set; }

            public int Size { get; set; }

            public string Color { get; set; }

            public decimal Price { get; set; }
        }

        public static void Seed()
        {
            using var session = DocumentStoreHolder.GetStore().Initialize().OpenSession();

            // Adidas
            session.Store(new Sneaker { Price = 117, Company = "Adidas", Brand = "Terex", Country = "Germany", Size = 45, Color = "White" });
            session.Store(new Sneaker { Price = 123, Company = "Adidas", Brand = "Terex", Country = "Germany", Size = 46, Color = "White" });
            session.Store(new Sneaker { Price = 100, Company = "Adidas", Brand = "Terex", Country = "Poland", Size = 46, Color = "White" });
            session.Store(new Sneaker { Price = 100, Company = "Adidas", Brand = "Superstar", Country = "Poland", Size = 45, Color = "Red" });
            session.Store(new Sneaker { Price = 100, Company = "Adidas", Brand = "Superstar", Country = "Poland", Size = 47, Color = "White" });

            // Nike
            session.Store(new Sneaker { Price = 157, Company = "Nike", Brand = "AirMax", Country = "Germany", Size = 45, Color = "White" });
            session.Store(new Sneaker { Price = 101, Company = "Nike", Brand = "AirMax", Country = "Germany", Size = 46, Color = "White" });
            session.Store(new Sneaker { Price = 100, Company = "Nike", Brand = "Flyknit", Country = "Poland", Size = 46, Color = "Blue" });
            session.Store(new Sneaker { Price = 100, Company = "Nike", Brand = "Flyknit", Country = "Poland", Size = 44, Color = "Red" });
            session.Store(new Sneaker { Price = 100, Company = "Nike", Brand = "Flyknit", Country = "Poland", Size = 47, Color = "White" });

            session.SaveChanges();
        }

        public static void Query()
        {
            using var session = DocumentStoreHolder.GetStore().Initialize().OpenSession();

            // Grouping by Company and Brand, counting 
            var results = session.Query<Sneaker>()
                .GroupBy(sneaker => new
                {
                    sneaker.Company,
                    sneaker.Brand
                })
                .Select(x => new
                {
                    Company = x.Key.Company,
                    Brand = x.Key.Brand,
                    Count = x.Count()
                })
                .ToList();

            AnsiConsole.Markup($"\n[black on yellow]Sneakers grouped by company and brand, counting[/]\n\n");
            foreach (var res in results)
            {
                AnsiConsole.WriteLine($" {res.Company} - {res.Brand} - {res.Count}");
            }

            // Grouping by Company and Brand, TotalPrice 
            var res2 = session.Query<Sneaker>()
                .GroupBy(sneaker => new
                {
                    sneaker.Company,
                    sneaker.Brand
                })
                .Select(x => new
                {
                    Company = x.Key.Company,
                    Brand = x.Key.Brand,
                    Count = x.Count(),
                    TotalPrice = x.Sum(a => a.Price),
                })
                .ToList();

            AnsiConsole.Markup($"\n[black on yellow]Sneakers grouped by company and brand, total price[/]\n\n");
            foreach (var res in res2)
            {
                AnsiConsole.WriteLine($" {res.Company} - {res.Brand} :: {res.Count} :: TotalPrice = {res.TotalPrice}");
            }

            // Grouping by Company and Brand, filtered by Country, average price
            var res3 = session.Query<Sneaker>()
                .Where(x => x.Country == "Germany")
                .GroupBy(sneaker => new
                {
                    sneaker.Company,
                    sneaker.Brand,
                    sneaker.Country
                })
                .Select(x => new
                {
                    Company = x.Key.Company,
                    Brand = x.Key.Brand,
                    Count = x.Count(),
                    TotalPrice = x.Sum(a => a.Price),
                })
                .ToList();

            AnsiConsole.Markup($"\n[black on yellow]Sneakers grouped by company and brand, filtered for Germany[/]\n\n");
            foreach (var res in res3)
            {
                AnsiConsole.WriteLine($" {res.Company} - {res.Brand} :: {res.Count} :: AveragePrice = {res.TotalPrice / res.Count}");
            }

            // Untyped grouping by Company and Brand, filtered by Country, average price
            var res4 = session.Advanced.DocumentQuery<Sneaker>()
                .WhereEquals("Country", "Germany")
                .GroupBy("Company", "Brand", "Country")
                .SelectKey("Company")
                .SelectKey("Brand")
                .SelectKey("Price")
                .SelectCount("Count")
                .OfType<dynamic>()
                .ToList();

            AnsiConsole.Markup($"\n[black on yellow]Sneakers grouped by company and brand, filtered for Germany[/]\n\n");
            foreach (var res in res4)
            {
                AnsiConsole.WriteLine($" {res.Company} - {res.Brand} :: {res.Count} :: AveragePrice = {res.Price / res.Count} ");
            }

            // Untyped grouping by Company and Brand, filtered by Country, average price
            string[] groupingFields = { "Company", "Brand", "Country" };
            string[] selectFields = { "Company", "Brand", "Price" };

            var filter = session.Advanced.DocumentQuery<Sneaker>()
                .WhereEquals("Country", "Germany");

            var grouping = filter
                .GroupBy("Company", "Brand", "Country");

            foreach (string field in selectFields)
                grouping = grouping.SelectKey(field);

            var res5 = grouping
                .SelectCount("Count")
                .OfType<dynamic>()
                .ToList();

            AnsiConsole.Markup($"\n[black on yellow]Sneakers grouped by company and brand, filtered for Germany[/]\n\n");
            foreach (var res in res5)
            {
                AnsiConsole.WriteLine($" {res.Company} - {res.Brand} :: {res.Count} :: AveragePrice = {res.Price / res.Count} ");
            }
        }
    }
}
