using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Northwind.Models.Entity;
using Raven.Client.Documents;

namespace Northwind.Features.Streaming
{
    public static class Streaming
    {
        public static void ToStream()
        {
            using (var stream = new MemoryStream())
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                session.Query<Employee>()
                    .Where(x => x.FirstName == "Robert")
                    .ToStream(stream);

                stream.Position = 0;
                var json = JObject.Load(new JsonTextReader(new StreamReader(stream)));
                var res = json.GetValue("Results");

                Console.WriteLine(res);
            }
        }
    }
}
