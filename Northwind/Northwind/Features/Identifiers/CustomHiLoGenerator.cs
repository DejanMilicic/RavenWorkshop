using System;
using Raven.Client.Documents;
using Raven.Client.Documents.Identity;
using static Raven.Client.Documents.Identity.AsyncHiLoIdGenerator;

namespace Northwind.Features.Identifiers;

public class User
{
    public string Id { get; set; }

    public string Name { get; set; }
}

public static class CustomHiLoGenerator
{
    public static void Demo()
    {
        #region Customize Store

        using var store = new DocumentStore { Urls = new[] { "http://127.0.0.1:8080" }, Database = "demo" };

        var gen = new SingleNodeAsyncMultiDatabaseHiLoIdGenerator(store);
        store.Conventions.AsyncDocumentIdGenerator = gen.GenerateDocumentIdAsync;

        store.Initialize();

        #endregion

        User user = new User { Name = "John" };

        using var session = store.OpenSession();
        session.Store(user);
        session.SaveChanges();

        Console.WriteLine(user.Id);
    }
}

public class SingleNodeAsyncMultiDatabaseHiLoIdGenerator : AsyncMultiDatabaseHiLoIdGenerator
{
    public SingleNodeAsyncMultiDatabaseHiLoIdGenerator(DocumentStore store) : base(store)
    {
    }

    public override AsyncMultiTypeHiLoIdGenerator GenerateAsyncMultiTypeHiLoFunc(string dbName)
    {
        return new SingleNodeAsyncMultiTypeHiLoIdGenerator(Store, dbName);
    }

    public class SingleNodeAsyncMultiTypeHiLoIdGenerator : AsyncMultiTypeHiLoIdGenerator
    {
        public SingleNodeAsyncMultiTypeHiLoIdGenerator(DocumentStore store, string dbName) : base(store, dbName)
        {
        }

        protected override AsyncHiLoIdGenerator CreateGeneratorFor(string tag)
        {
            return new SingleNodeAsyncHiLoIdGenerator(tag, Store, DbName, Conventions.IdentityPartsSeparator);
        }

        public class SingleNodeAsyncHiLoIdGenerator : AsyncHiLoIdGenerator
        {
            public SingleNodeAsyncHiLoIdGenerator(string tag, DocumentStore store, string dbName, char identityPartsSeparator)
                : base(tag, store, dbName, identityPartsSeparator)
            {
            }

            protected override string GetDocumentIdFromId(NextId nextId)
            {
                return $"{Prefix}{nextId.Id}";
            }
        }
    }
}
