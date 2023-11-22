using System;
using System.Collections.Generic;
using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents.Operations;

namespace Northwind.Features.Patching;

public static class Patching
{
    public static void PatchFromRevision()
    {
        string orderId = "orders/830-A";

        using var session = DocumentStoreHolder.Store.OpenSession();

        List<Order> revisions = session.Advanced.Revisions.GetFor<Order>(orderId).ToList();

        var revisionDate = revisions.Skip(1).First().OrderedAt;
        session.Advanced.Patch<Order, DateTime>(orderId, x => x.OrderedAt, revisionDate);
        
        session.SaveChanges();
    }

    /// <docs>
    /// https://ravendb.net/docs/article-page/latest/Csharp/client-api/operations/patching/set-based#patching-how-to-perform-set-based-operations-on-documents
    /// </docs>
    public static void PatchByQuery()
    {
        using var session = DocumentStoreHolder.Store.OpenSession();

        // set discount to all orders that was processed by a specific employee
        var operation = DocumentStoreHolder.Store
            .Operations
            .Send(new PatchByQueryOperation(@"
                from Orders as o
                where o.Employee = 'employees/4-A'
                update
                {
                  o.Lines.forEach(line=> line.Discount = 0.3);
                }
            "));

        // Wait for the operation to complete on the server side.
        // Not waiting for completion will not harm the patch process and it will continue running to completion.
        operation.WaitForCompletion();
    }
}
