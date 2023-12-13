using System.Collections.Generic;
using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents.Session;
using Raven.Client.Documents;
using System;

namespace Northwind.Features.Bugs;

public static class Bugs
{
    public static void Include()
    {
        using var session = DocumentStoreHolder.GetStore().Initialize().OpenSession(new SessionOptions { NoTracking = true });

        List<Order> londonOrders = session
            .Query<Order>()
            .Include(o => o.Company)
            .Where(o => o.ShipTo.City == "London")
            .ToList();

        Company company = session.Load<Company>("companies/11-A");

        Console.WriteLine($"Total requests: {session.Advanced.NumberOfRequests}");

        // Why is this code producing two calls to the server, instead of just one?
        // Aren't we using Include() to avoid that?
        //
        // We are using NoTracking = true
        // and Include() is not supported in that mode
    }
}
