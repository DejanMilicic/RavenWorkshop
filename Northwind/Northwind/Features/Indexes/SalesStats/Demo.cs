using System;
using System.Collections.Generic;
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
            Timestamp = new DateTime(2024, 10, 15, 5, 1, 1),
            RecievedAmount = 0,
            PaidAmount = 78,
            LoadStatus = "Done"
        });

        session.Store(new SalesRecord
        {
            User = "Georg",
            Timestamp = new DateTime(2024, 11, 15, 1, 1, 1),
            RecievedAmount = 110,
            PaidAmount = 0,
            LoadStatus = "In Progress"
        });

        session.Store(new SalesRecord
        {
            User = "Martha",
            Timestamp = new DateTime(2024, 10, 15, 5, 1, 1),
            RecievedAmount = 5680,
            PaidAmount = 0,
            LoadStatus = "Done"
        });

        session.Store(new SalesRecord
        {
            User = "Greg",
            Timestamp = new DateTime(2024, 10, 15, 5, 1, 1),
            RecievedAmount = 0,
            PaidAmount = 333,
            LoadStatus = "Done"
        });

        session.SaveChanges();
    }

    private static void PrintResults(string heading, List<Sales_Aggregations.Entry> results)
    {
        Console.WriteLine("\n" + heading);

        foreach (var result in results)
        {
            Console.WriteLine($"User: {result.User}, Timeframe: {result.Timeframe}, Timestamp: {result.Timestamp}, Receivables: {result.Receivables}, Payables: {result.Payables}, Balance: {result.Balance}, Margin: {result.Margin}%");
        }
    }

    public static void Query()
    {
        using var session = GetStore().OpenSession();

        PrintResults("Sales aggregations for \"Georg\" in 4th Quarter of 2024",
            session.Query<Sales_Aggregations.Entry, Sales_Aggregations>()
            .Where(x => x.User == "Georg" && x.Timeframe == "Quarter" && x.Timestamp == "2024-4")
            .ToList());

        PrintResults("Sales aggregations for October of 2024, ordered by Receivables aggregation",
            session.Query<Sales_Aggregations.Entry, Sales_Aggregations>()
            .Where(x => x.Timeframe == "Month" && x.Timestamp == "2024-10")
            .OrderByDescending(x => x.Receivables)
            .ToList());
    }
}
