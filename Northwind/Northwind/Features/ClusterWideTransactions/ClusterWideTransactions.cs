using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;

namespace Northwind.Features.ClusterWideTransactions
{
    public class ClusterWideTransactions
    {
        public async Task Do()
        {
            using (var session = DocumentStoreHolder.Store.OpenAsyncSession(
                new SessionOptions { TransactionMode = TransactionMode.ClusterWide }))
            {
                var cmpxchg = session.Advanced.ClusterTransaction
                    .CreateCompareExchangeValue("test/1", new object());

                await session.SaveChangesAsync();
            }

            using (var session = DocumentStoreHolder.Store.OpenAsyncSession(
                new SessionOptions { TransactionMode = TransactionMode.ClusterWide }))
            {
                var cmpxchg = session.Advanced.ClusterTransaction
                    .CreateCompareExchangeValue("test/1", new object());

                await session.StoreAsync(new object(), "o/1");

                try
                {
                    await session.SaveChangesAsync();
                }
                catch (ConcurrencyException)
                {
                    cmpxchg = session.Advanced.ClusterTransaction
                        .CreateCompareExchangeValue("test/2", new object());

                    await session.SaveChangesAsync();
                }
            }
		}
    }
}
