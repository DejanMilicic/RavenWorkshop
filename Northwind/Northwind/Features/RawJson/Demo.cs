using Newtonsoft.Json.Linq;
using Raven.Client.Documents.Commands.Batches;
using Raven.Client.Documents.Session;
using Sparrow.Json.Parsing;

namespace Northwind.Features.RawJson;

public class JsonHelper
{
    public static DynamicJsonValue ConvertJObjectToDynamicJsonValue(JObject jObject)
    {
        var djv = new DynamicJsonValue();

        foreach (var property in jObject.Properties())
        {
            var value = property.Value;

            if (value is JObject)
                djv[property.Name] = ConvertJObjectToDynamicJsonValue((JObject)value);
            else if (value is JArray)
                djv[property.Name] = ConvertJArrayToDynamicJsonArray((JArray)value);
            else
                djv[property.Name] = value.ToObject<object>();
        }

        return djv;
    }

    private static DynamicJsonArray ConvertJArrayToDynamicJsonArray(JArray jArray)
    {
        DynamicJsonArray dja = new DynamicJsonArray();

        foreach (var item in jArray)
        {
            if (item is JObject)
                dja.Add(ConvertJObjectToDynamicJsonValue((JObject)item));
            else if (item is JArray)
                dja.Add(ConvertJArrayToDynamicJsonArray((JArray)item));
            else
                dja.Add(item.ToObject<object>());
        }

        return dja;
    }
}

public static class RawJson
{
    public static void Demo2(IDocumentSession session, string id, string collection, string json)
    {
        JObject parsedJson = JObject.Parse(json);

        DynamicJsonValue djv = JsonHelper.ConvertJObjectToDynamicJsonValue(parsedJson);
        djv["@metadata"] = new DynamicJsonValue { ["@collection"] = collection };
        
        session.Advanced.Defer(new PutCommandData(id, null, djv));
    }
}
