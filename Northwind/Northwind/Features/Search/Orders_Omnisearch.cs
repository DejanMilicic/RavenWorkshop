using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Search
{
    public class Orders_Omnisearch : AbstractMultiMapIndexCreationTask<Orders_Omnisearch.Entry>
    {
        public class Entry
        {
            public string[] Query { get; set; }
        }

        public Orders_Omnisearch()
        {
            AddMap<Order>(orders => from order in orders
                from orderLine in order.Lines

                let product = LoadDocument<Product>(orderLine.Product)
                let category = LoadDocument<Category>(product.Category)
                let supplier = LoadDocument<Supplier>(product.Supplier)

                select new Entry
                {
                    Query = new[]
                    {
                        orderLine.ProductName,
                        product.QuantityPerUnit,
                        category.Name,
                        supplier.Name
                    }
                }
            );

            AddMap<Order>(orders => from order in orders

                let company = LoadDocument<Company>(order.Company)
                let employee = LoadDocument<Employee>(order.Employee)

                select new Entry
                {
                    Query = new[]
                    {
                        company.Name, 
                        company.Address.City,
                        company.Address.Country,
                        employee.FirstName,
                        employee.LastName
                    }
                }
            );

            Index(x => x.Query, FieldIndexing.Search);
        }
    }
}
