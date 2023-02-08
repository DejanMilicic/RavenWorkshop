using System;

namespace Northwind.Features.Changes
{
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
    }
}
