using System;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace Northwind.Features.Operations
{
    public class Operations
    {
        public static void CreateDatabase()
        {
            var store = DocumentStoreHolder.Store;

            store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord("sample_" + Guid.NewGuid())));
        }
    }
}
