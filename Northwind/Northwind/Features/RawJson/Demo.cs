using System.IO;
using System.Text;
using Raven.Client.Documents.Commands.Batches;
using Raven.Client.Documents.Session;
using SpanJson;
using Sparrow.Json;
using Sparrow.Json.Parsing;

namespace Northwind.Features.RawJson;

public static class RawJson
{
    /*
    public static void Demo()
    {
        using var session = DocumentStoreHolder.Store.OpenSession();

        dynamic json = new
        {
            Name = "My Product",
            Supplier = "suppliers/1-A",
        };

        //json["@metadata"] = new DynamicJsonValue
        //{
        //    ["@collection"] = "Products"
        //};

        var jsonData = JsonSerializer.Generic.Utf16.Serialize(json);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));
        using var context = JsonOperationContext.ShortTermSingleUse();
        BlittableJsonReaderObject bjro = context.ReadForMemoryAsync(stream, "products/999-A").Result;

        session.Advanced.Defer(
            new PutCommandData("products/999-A", null, new DynamicJsonValue(bjro))
        );

        // All deferred commands will be sent to the server upon calling SaveChanges
        session.SaveChanges();
    }
    */

    public static void Demo2(IDocumentSession session, string id, string collection, string json)
    {
        //JObject jobject = JObject.Parse(json);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        using var context = JsonOperationContext.ShortTermSingleUse();
        BlittableJsonReaderObject bjro = context.ReadForMemoryAsync(stream, id).Result;

        var djv = new DynamicJsonValue(bjro);
        djv["@metadata"] = new DynamicJsonValue { ["@collection"] = "Products" };
        session.Advanced.Defer(new PutCommandData(id, null, djv));

        session.SaveChanges();
    }
}
