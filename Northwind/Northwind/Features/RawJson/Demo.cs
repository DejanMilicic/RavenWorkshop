using Raven.Client.Documents.Session;
using System.IO;
using System.Collections.Generic;

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

    public static string GetRawJson1(IDocumentSession session, string id)
    {
        return session.Load<Newtonsoft.Json.Linq.JObject>(id).ToString(Newtonsoft.Json.Formatting.None);
    }

    public static string GetRawJson2(IDocumentSession session, string id)
    {
        using (var memoryStream = new MemoryStream())
        {
            session.Advanced.LoadIntoStream(new List<string>{ id }, memoryStream);
            memoryStream.Position = 0; // Reset the stream position to the beginning
            using (var reader = new StreamReader(memoryStream, System.Text.Encoding.UTF8))
            {
                string json = reader.ReadToEnd();
                var jsonDocument = System.Text.Json.JsonDocument.Parse(json);
                var result = jsonDocument.RootElement.GetProperty("Results")[0].ToString();

                var resultObject = System.Text.Json.Nodes.JsonNode.Parse(result) as System.Text.Json.Nodes.JsonObject;

                resultObject.Remove("@metadata");

                return resultObject.ToJsonString(new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            }
        }
    }
}