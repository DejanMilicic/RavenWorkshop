using Raven.Client.Documents.Session;

namespace Northwind.Features.RawJson;

public static class RawJson
{
    public static void StoreRawJson1(IDocumentSession session, string id, string collection, string json)
    {
        var parsedJson = Newtonsoft.Json.Linq.JObject.Parse(json);
        parsedJson["@metadata"] = new Newtonsoft.Json.Linq.JObject { ["@collection"] = collection };
        session.Store(parsedJson, id);
    }

    public static void StoreRawJson2(IDocumentSession session, string id, string collection, string json)
    {
        var parsedJson = System.Text.Json.Nodes.JsonNode.Parse(json);
        parsedJson["@metadata"] = new System.Text.Json.Nodes.JsonObject { ["@collection"] = collection };
        session.Store(parsedJson, id);
    }

    public static string GetRawJson(IDocumentSession session, string id)
    {
        return session.Load<Newtonsoft.Json.Linq.JObject>(id).ToString(Newtonsoft.Json.Formatting.None);
    }
}