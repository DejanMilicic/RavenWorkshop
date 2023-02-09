using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents;

namespace Northwind.Features.Streaming
{
    public static class JsonStreaming
    {
        public static void ToStream()
        {
            using (var stream = new MemoryStream())
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                session
                    .Query<Order>()
                    .ToStream(stream);

                stream.Position = 0;
                var json = JObject.Load(new JsonTextReader(new StreamReader(stream)));
                var res = json.GetValue("Results");

                Console.WriteLine(res);
            }
        }
    }
}
