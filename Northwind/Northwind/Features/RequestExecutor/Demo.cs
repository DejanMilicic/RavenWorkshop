using System;
using Raven.Client.Http;

namespace Northwind.Features.RequestExecutor
{
    public static class RequestExecutorDemo
    {
        public static void GetStatistics()
        {
            var store = DocumentStoreHolder.Store;

            store.GetRequestExecutor().Print();
        }

        public static void Print(this Raven.Client.Http.RequestExecutor re)
        {
            Console.WriteLine("# Request Executor");
            Console.WriteLine($"\t Url: {re.Url}");

            re.Topology.Print();
        }

        public static void Print(this Topology t)
        {
            Console.WriteLine("## Topology");
            Console.WriteLine();
            Console.WriteLine($"ETag: {t.Etag}");
            Console.WriteLine("Nodes:");
            foreach (ServerNode node in t.Nodes)
            {
                Console.WriteLine($"\t{node.ClusterTag} : {node.Url}");
            }
        }
    }
}
