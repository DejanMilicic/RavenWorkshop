using System;
using System.Collections.Generic;
using System.Dynamic;
using Raven.Client.Documents;

namespace Northwind.Samples.JsonObject
{
    public static class JsonObjectStoreHolder
    {
        private static readonly Lazy<IDocumentStore> LazyStore =
            new Lazy<IDocumentStore>(() =>
            {
                IDocumentStore store = new DocumentStore
                {
                    Urls = new[] { "http://127.0.0.1:8080" },
                    Database = "jsonobjects"
                };

                store.Initialize();

                return store;
            });

        public static IDocumentStore Store => LazyStore.Value;
    }

    public static class JsonObjects
    {
        public static void Seed()
        {
            using var session = JsonObjectStoreHolder.Store.OpenSession();

            dynamic json1 = new ExpandoObject();
            json1.id = "abc1";
            json1.title = "title1";
            json1.sys_created = DateTime.UtcNow;
            json1.p1 = "v1";
            json1.p2 = "v2";
            json1.p3 = "v3";
            session.Store(json1);

            dynamic json2 = new ExpandoObject();
            json2.id = "ident1";
            json2.title = "title2";
            json2.sys_created = DateTime.UtcNow;
            json2.a1 = "b1";
            json2.a2 = "b2";
            json2.a3 = "b3";
            session.Store(json2);

            session.SaveChanges();
        }

        public static void Query()
        {
            using var session = JsonObjectStoreHolder.Store.OpenSession();

            List<dynamic> list = session.Advanced.DocumentQuery<dynamic>()
                .WhereStartsWith("title", "ti")
                .WhereLessThan("sys_created", DateTimeOffset.UtcNow)
                .OrderBy("id")
                .ToList();
        }
    }
}
