using System;

namespace Northwind.Features.Changes;

public static class Changes
{
    public static void Consume()
    {
        DocumentStoreHolder.Store.Changes()
            .ForAllDocuments()
            .Subscribe(change =>
            {
                Console.WriteLine($"{change.Id} {change.Type}");
            });

        Console.ReadLine();
    }

    // TODO: examples for other cases (collection, index, ...)
    // https://ravendb.net/docs/article-page/5.4/Csharp/client-api/changes/what-is-changes-api
}
