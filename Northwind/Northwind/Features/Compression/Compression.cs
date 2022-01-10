using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations.DocumentsCompression;

namespace Northwind.Features.Compression
{
    public class Compression
    {
        public static void TurnCompressionOn()
        {
            var store = DocumentStoreHolder.Store;
            store.Maintenance.Send(
                new UpdateDocumentsCompressionConfigurationOperation(
                    new DocumentsCompressionConfiguration(
                        compressRevisions: true,
                        compressAllCollections: true
                        )));
        }

        public static void TurnCompressionOnForTwoCollections()
        {
            var store = DocumentStoreHolder.Store;
            store.Maintenance.Send(
                new UpdateDocumentsCompressionConfigurationOperation(
                    new DocumentsCompressionConfiguration(
                        compressRevisions: true,
                        collections: new string[]{ "Employees", "Orders" }
                        )));
        }
    }
}
