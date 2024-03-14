using System;
using Microsoft.AspNetCore.JsonPatch;
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
    public static void DeferredInterpolatedPatch()
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

    /// <summary>
    /// Parametrized deferred patch is always better, since it is being cached and reused by the server.
    /// </summary>
    public static void DeferredParametrizedPatch()
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
                    Script = "this['@metadata']['@expires'] = args.expires;", // args is Values collection
                    Values =
                    {
                        ["expires"] = DateTime.UtcNow.AddMinutes(1).ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ")
                    }

                    // alternative syntax
                    // Values = { { "comment1", comment } }

                    // one more alternative syntax
                    // Script = "this.LastName = args.newUser.LastName;",
                    // Values =
                    // {
                    //    {"newUser", new { LastName = "Yitzchaki" }}
                    // }
                }
            )
        );

        session.SaveChanges();
    }

    /// <summary>
    /// JSON Patch Syntax - https://ravendb.net/docs/article-page/latest/csharp/client-api/operations/patching/json-patch-syntax
    ///
    /// This is fastest of all options.
    /// JS patch needs to convert JSON to JS object, execute JS and rebuild JSON again.
    /// JSON Patch can traverse JSON directly, which is much more efficient.
    /// </summary>
    public static void JsonPatch()
    {
        using var session = DocumentStoreHolder.Store.OpenSession();

        session.Store(new User { Email = "soon.to.be@deleted.com" }, "users/1");
        session.SaveChanges();

        var patchesDocument = new JsonPatchDocument();
        patchesDocument.Add("/@metadata/@expires", DateTime.UtcNow.AddMinutes(1).ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"));
        DocumentStoreHolder.Store.Operations.Send(new JsonPatchOperation("users/1", patchesDocument));
    }
}
