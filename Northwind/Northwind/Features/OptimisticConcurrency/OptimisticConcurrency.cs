using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Models.Entity;
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
    }
}
