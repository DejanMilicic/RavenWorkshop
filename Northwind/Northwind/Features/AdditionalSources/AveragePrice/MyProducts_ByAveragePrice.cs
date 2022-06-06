using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Features.Attachments;
using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.AdditionalSources.AveragePrice
{
    public class MyProducts_ByAveragePrice : AbstractIndexCreationTask<MyProductPriceHistory, MyProducts_ByAveragePrice.Entry>
    {
        public class Entry
        {
            public string ProductId { get; set; }

            public string ProductName { get; set; }

            public decimal AveragePrice { get; set; }
        }

        public MyProducts_ByAveragePrice()
        {
            Map = phs => from ph in phs
                let product = LoadDocument<MyProduct>(ph.ProductId)
                select new Entry
                {
                    ProductId = ph.ProductId,
                    ProductName = product.Name,
                    AveragePrice = ComputeAveragePrice.FromPriceHistory(ph.Entries.Select(x => (decimal)x.Price).ToList())
                };

            AdditionalSources = new Dictionary<string, string>
            {
                ["ComputeAveragePrice.cs"] =
                    File.ReadAllText(Path.Combine(new[] 
                        { AppContext.BaseDirectory, "..", "..", "..", "Features", "AdditionalSources", "AveragePrice", "ComputeAveragePrice.cs" }))
            };
        }
    }
}
