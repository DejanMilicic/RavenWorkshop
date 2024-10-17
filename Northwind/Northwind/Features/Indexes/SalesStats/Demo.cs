using System;
using System.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Indexes.SalesStats;

public static class SalesStats
{
    private static IDocumentStore GetStore()
    {
        var store = (new DocumentStore
        {
            Urls = new[] { "http://127.0.0.1:8080" },
            Database = "Sales"
        }).Initialize();

        IndexCreation.CreateIndexes(new AbstractIndexCreationTask[] { new Sales_Aggregations() }, store);

        return store;
    }

    public static void Seed()
    {
        using var session = GetStore().OpenSession();

        session.Store(new SalesRecord
        {
            User = "Georg",
            Timestamp = new DateTime(2024, 10, 15, 1, 1, 1),
            RecievedAmount = 100,
            PaidAmount = 0,
            LoadStatus = "Done"
        });

        session.Store(new SalesRecord
        {
            User = "Georg",
            Timestamp = new DateTime(2024, 11, 15, 1, 1, 1),
            RecievedAmount = 110,
            PaidAmount = 0,
            LoadStatus = "Done"
        });

        session.SaveChanges();
    }
}
