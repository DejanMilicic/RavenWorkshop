using Raven.Client.Documents.Indexes;
using Raven.Client.Documents;

namespace Northwind.Features.InnerJoin
{
    public static class InnerJoin
    {
        public static void Demo()
        {
            var store = new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "Transport"
            }.Initialize();

            IndexCreation.CreateIndexes(new AbstractIndexCreationTask[] { new Vehicles() }, store);

            // uncomment to seed
            //Seed.Data(store);
        }
    }
}
