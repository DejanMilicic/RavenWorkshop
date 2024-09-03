using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Raven.Client.Documents.Commands.Batches;
using Raven.Client.Documents.Session;
using Sparrow.Json.Parsing;

namespace Northwind.Features.RawJson;

public class JsonHelper
{
    public static Dictionary<string, object> ConvertJObjectToDictionary(JObject jObject)
    {
        var dictionary = new Dictionary<string, object>();

        foreach (var property in jObject.Properties())
        {
            var value = property.Value;

            if (value is JObject)
            {
                // Recursively convert nested JObject to Dictionary<string, object>
                dictionary[property.Name] = ConvertJObjectToDictionary((JObject)value);
            }
            else if (value is JArray)
            {
                // Convert JArray to List<object>
                dictionary[property.Name] = ConvertJArrayToList((JArray)value);
            }
            else
            {
                // Add the value as is for simple types
                dictionary[property.Name] = value.ToObject<object>();
            }
        }

        return dictionary;
    }

    private static List<object> ConvertJArrayToList(JArray jArray)
    {
        var list = new List<object>();

        foreach (var item in jArray)
        {
            if (item is JObject)
            {
                // Recursively convert nested JObject to Dictionary<string, object>
                list.Add(ConvertJObjectToDictionary((JObject)item));
            }
            else if (item is JArray)
            {
                // Recursively convert nested JArray to List<object>
                list.Add(ConvertJArrayToList((JArray)item));
            }
            else
            {
                // Add the item as is for simple types
                list.Add(item.ToObject<object>());
            }
        }

        return list;
    }

    public static DynamicJsonValue ConvertToDynamicJsonValue(JObject jObject)
    {
        var dynamicJson = new DynamicJsonValue();

        foreach (var property in jObject.Properties())
        {
            var value = property.Value;

            if (value is JObject)
            {
                // Recursively convert nested JObject to DynamicJsonValue
                dynamicJson[property.Name] = ConvertToDynamicJsonValue((JObject)value);
            }
            else if (value is JArray)
            {
                // Convert JArray to object[]
                dynamicJson[property.Name] = ConvertJArrayToObjectArray((JArray)value);
            }
            else
            {
                // Add the value as is for simple types
                dynamicJson[property.Name] = value.ToObject<object>();
            }
        }

        return dynamicJson;
    }

    static object[] ConvertJArrayToObjectArray(JArray jArray)
    {
        var list = new object[jArray.Count];

        for (int i = 0; i < jArray.Count; i++)
        {
            var item = jArray[i];

            if (item is JObject)
            {
                // Recursively convert nested JObject to DynamicJsonValue
                list[i] = ConvertToDynamicJsonValue((JObject)item);
            }
            else if (item is JArray)
            {
                // Recursively convert nested JArray to object[]
                list[i] = ConvertJArrayToObjectArray((JArray)item);
            }
            else
            {
                // Add the item as is for simple types
                list[i] = item.ToObject<object>();
            }
        }

        return list;
    }
}

public static class RawJson
{
    public static void Demo2(IDocumentSession session, string id, string collection, string json)
    {
        JObject parsedJson = JObject.Parse(json);

        DynamicJsonValue djv = JsonHelper.ConvertToDynamicJsonValue(parsedJson);
        djv["@metadata"] = new DynamicJsonValue { ["@collection"] = collection };
        
        session.Advanced.Defer(new PutCommandData(id, null, djv));
    }
}
