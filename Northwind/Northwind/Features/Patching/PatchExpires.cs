using System;
using Raven.Client.Documents.Commands.Batches;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Queries;

namespace Northwind.Features.Patching;

public static class PatchExpires
{
    /// <summary>
    /// Query will be executed on the server side and will update all documents that match the query.
    ///
    /// You can either use QueryParameters or construct query directly with string interpolation.
    /// However, whenever you can, use QueryParameters since such parametrized queries
    /// are being cached and reused by the server.
    ///
    /// This operation will be executed immediately, and you do not have to wait for it to complete.
    /// </summary>
    public static void PatchByQuery()
    {
        using (var session = DocumentStoreHolder.Store.OpenSession())
        {
            session.Store(new User { Email = "soon.to.be@deleted.com" }, "users/1");
            session.SaveChanges();
        }

        DocumentStoreHolder.Store
            .Operations
            .Send(new PatchByQueryOperation(new IndexQuery
            {
                Query = $@"
                            from Users
                            update
                            {{
                                this['@metadata']['@expires'] = $timestamp;
                            }}
                        ",
                QueryParameters = new Raven.Client.Parameters()
                {
                    {"timestamp", DateTime.UtcNow.AddMinutes(1).ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ") }
                }
            }));
    }

    /// <summary>
    /// Deferred patch will be executed on the server side when SaveChanges is called.
    /// This is recommended approach for patching individual documents.
    /// Multiple patches can be deferred and executed in a single request.
    /// </summary>
    public static void DeferredPatch()
    {
        using var session = DocumentStoreHolder.Store.OpenSession();

        session.Store(new User { Email = "soon.to.be@deleted.com" }, "users/1");
        session.SaveChanges();

        session.Advanced.Defer(
            new PatchCommandData(
                "users/1", 
                null, 
                new PatchRequest
                {
                    Script = $"this['@metadata']['@expires'] = '{DateTime.UtcNow.AddMinutes(1).ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ")}'"
                }),
        null);

        session.SaveChanges();
    }
}
