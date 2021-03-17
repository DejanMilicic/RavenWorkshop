using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
using Raven.Client.Documents.Commands.Batches;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;

namespace Northwind.Features.OptimisticConcurrency
{
    public class OptimisticConcurrency
    {
        // last write wins
        public void Default()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();
            var order = session.Load<Order>("orders/1-A");
            order.Freight++; // breakpoint
            session.SaveChanges();
        }

        // optimistic concurrency on
        public void UseOptimisticConcurrency()
        {
            using var session = DocumentStoreHolder.Store.OpenSession();
            session.Advanced.UseOptimisticConcurrency = true;
            var order = session.Load<Order>("orders/1-A");
            order.Freight++; // breakpoint
            session.SaveChanges();
        }

        // optimistic concurrency on, with try/catch
        public void UseOptimisticConcurrencyWithCatch()
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

        // use patch to update single property
        public void UsePatch()
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

        public void UseDeferredPatch()
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
}
