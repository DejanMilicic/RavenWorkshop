using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Commands;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using Sparrow.Json;

namespace Northwind.Features.Operations
{
    public class Operations
    {
        public static void CreateDatabase()
        {
            var store = DocumentStoreHolder.Store;

            store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord("sample_" + Guid.NewGuid())));
        }

        public static void InsertJson()
        {
            using var store = new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "demo"
            }.Initialize();

            string id = "test/1";
            var json = @"{'Name':'John','Age':30}";

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            using var context = JsonOperationContext.ShortTermSingleUse();
            BlittableJsonReaderObject bjro = context.ReadForMemoryAsync(stream, id).Result;
            var c = new PutDocumentCommand(store.Conventions, id, "", bjro); // or store.GetRequestExecutor().Conventions

            store.GetRequestExecutor().Execute(c, context);
        }

        public static void InsertJson2()
        {
            var json = @"{""Name"":""John"",""Age"":30}";

            using var store = new DocumentStore
            {
                Urls = new[] { "http://127.0.0.1:8080" },
                Database = "demo",
                Conventions =
                {
                    FindClrTypeNameForDynamic = entity => "TeamMembers",
                    FindCollectionNameForDynamic = entity => "TeamMembers"
                }
            }.Initialize();

            using var session = store.OpenSession();
            JObject entity = JObject.Parse(json);
            session.Store(entity, "TeamMembers/John");
            session.SaveChanges();
        }
    }
}
