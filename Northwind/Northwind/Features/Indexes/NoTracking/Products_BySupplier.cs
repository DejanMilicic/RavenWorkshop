using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Indexes.NoTracking
{
    // Ability to index referenced documents without establishing tracking reference
    // if references are not tracked, changes in referenced documents WILL NOT trigger reindexing

    public class Products_BySupplier : AbstractIndexCreationTask<Product>
    {
        public Products_BySupplier()
        {
            Map = products => from product in products
                let supplier = LoadDocument<Supplier>(product.Supplier) // <--- here
                select new
                {
                    Name = supplier.Name
                };
        }
    }

    public class Products_BySupplier_NoTracking : AbstractIndexCreationTask<Product>
    {
        public Products_BySupplier_NoTracking()
        {
            Map = products => from product in products
                let supplier = NoTracking.LoadDocument<Supplier>(product.Supplier) // <--- here
                select new
                {
                    Name = supplier.Name
                };
        }
    }
}
