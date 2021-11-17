using System;
using System.Collections.Generic;
using System.Linq;
using Northwind.Models.Entity;

namespace Northwind.Features.Patching
{
    public class Patching
    {
        public void PatchFromRevision()
        {
            string orderId = "orders/830-A";

            using var session = DocumentStoreHolder.Store.OpenSession();
            List<Order> revisions = session.Advanced.Revisions.GetFor<Order>(orderId).ToList();

            var revisionDate = revisions.Skip(1).First().OrderedAt;
            session.Advanced.Patch<Order, DateTime>(orderId, x => x.OrderedAt, revisionDate);
            
            session.SaveChanges();
        }
    }
}
