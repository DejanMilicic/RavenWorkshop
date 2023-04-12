using System;
using Northwind.Models.Entity;
using Raven.Client.Documents.Commands.Batches;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions;

namespace Northwind.Features.OptimisticConcurrency;

public static class OptimisticConcurrency
{
    // last write wins
    public static void Default()
    {
        using var session = DocumentStoreHolder.Store.OpenSession();
        var order = session.Load<Order>("orders/1-A");
        order.Freight++; // breakpoint
        session.SaveChanges();
    }

    // optimistic concurrency on
    public static void UseOptimisticConcurrency()
    {
        using var session = DocumentStoreHolder.Store.OpenSession();
        session.Advanced.UseOptimisticConcurrency = true; // <--- this line added
        var order = session.Load<Order>("orders/1-A");
        order.Freight++; // breakpoint
        session.SaveChanges();
    }

    // optimistic concurrency on, with try/catch
    public static void UseOptimisticConcurrencyWithCatch()
    {
        try
        {
            using var session = DocumentStoreHolder.Store.OpenSession();
            session.Advanced.UseOptimisticConcurrency = true;
            var order = session.Load<Order>("orders/1-A");
            order.Freight++; // breakpoint
            session.SaveChanges();
        }
        catch (ConcurrencyException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static void UseChangeVector()
    {
        using var session = DocumentStoreHolder.Store.OpenSession();

        Employee laura2 = new Employee();
        laura2.Id = "employees/8-A";
        laura2.LastName += " CHANGED";

        // force concurrency check
        session.Store(laura2, changeVector: string.Empty, id: laura2.Id);

        session.SaveChanges();
    }

    // use patch to update single property
    public static void UsePatch()
    {
        try
        {
            using var session = DocumentStoreHolder.Store.OpenSession();
            session.Advanced.UseOptimisticConcurrency = true;
            var order = session.Load<Order>("orders/1-A");
            //order.Freight++; // breakpoint
            session.Advanced.Patch(order, o => o.Freight, order.Freight+1);
            session.SaveChanges();
        }
        catch (ConcurrencyException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    // TODO
    //from "Employees" update {
    //    this.Name = this.FirstName + " " + this.LastName;        
    //}

    public static void UseDeferredPatch()
    {
        try
        {
            using var session = DocumentStoreHolder.Store.OpenSession();
            session.Advanced.UseOptimisticConcurrency = true;
            var order = session.Load<Order>("orders/1-A");
            //order.Freight++; // breakpoint
            session.Advanced.Patch(order, o => o.Freight, order.Freight+1);

            session.Advanced.Defer(new PatchCommandData("orders/1-A", null, 
                new PatchRequest
                {
                    Script = "this.Freight++;"
                }, null));

            session.SaveChanges();
        }
        catch (ConcurrencyException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

