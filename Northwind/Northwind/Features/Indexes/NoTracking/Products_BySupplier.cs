using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Indexes.NoTracking
{
    // Ability to index referenced documents without establishing tracking reference
    // if references are not tracked, changes in referenced documents WILL NOT trigger reindexing
    // this will save CPU but it also may result in index entries which are inaccurate
    // i.e. index state may not reflect current state of data

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
