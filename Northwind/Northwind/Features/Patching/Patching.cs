using System;
using System.Collections.Generic;
using System.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents.Commands.Batches;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions;

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

    public static void OptimisticConcurrencyPatch()
    {
        using var session = DocumentStoreHolder.Store.OpenSession();
        session.Advanced.UseOptimisticConcurrency = true;

        string id = "employees/8-A";

        var emp = session.Load<Employee>(id);

        // after loading document, you can construct patch script based on loaded document

        string changeVector = session.Advanced.GetChangeVectorFor(emp);

        session.Advanced.Defer(new BatchPatchCommandData(
            new List<(string, string)>{("employees/8-A", changeVector)}, 
            new PatchRequest
            {
                Script = "this.Age = 100;"
            }, 
            null));

        try
        {
            session.SaveChanges();
        }
        catch (ConcurrencyException e)
        {
            // document was changed since you loaded it
        }
    }
}
