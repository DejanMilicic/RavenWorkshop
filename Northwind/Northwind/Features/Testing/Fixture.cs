using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.TestDriver;
using System.Collections.Generic;

namespace Northwind.Features.Testing
{
    public class Fixture : RavenTestDriver
    {
        protected readonly IDocumentStore Store;
        protected Dictionary<string, int> SessionsRecorded;

        static Fixture()
        {
            var testServerOptions = new TestServerOptions
            {
                FrameworkVersion = null // user latest, optional
            };

            ConfigureServer(testServerOptions);
        }

        public Fixture()
        {
            Store = this.GetDocumentStore();
            IndexCreation.CreateIndexes(typeof(Program).Assembly, Store);

            SessionsRecorded = new Dictionary<string, int>();

            Store.OnSessionDisposing += (sender, args) =>
            {
                string sessionId = args.Session.Id.ToString();

                if (SessionsRecorded.ContainsKey(sessionId))
                    SessionsRecorded[sessionId] = args.Session.NumberOfRequests;
                else
                    SessionsRecorded.Add(sessionId, args.Session.NumberOfRequests);
            };
        }
    }
}
