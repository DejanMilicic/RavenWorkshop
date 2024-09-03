using Newtonsoft.Json.Linq;
using Raven.Client.Documents.Session;
using Newtonsoft.Json;

namespace Northwind.Features.RawJson;

public static class RawJson
{
    public static void StoreRawJson(IDocumentSession session, string id, string collection, string json)
    {
        JObject parsedJson = JObject.Parse(json);
        parsedJson["@metadata"] = new JObject { ["@collection"] = collection };
        session.Store(parsedJson, id);
    }

    public static string GetRawJson(IDocumentSession session, string id)
    {
        return session.Load<JObject>(id).ToString(Formatting.None);
    }
}