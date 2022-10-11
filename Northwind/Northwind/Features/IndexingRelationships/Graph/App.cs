using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.IndexingRelationships.Graph
{
    public static class App
    {
        public static void Execute()
        {
            //Seed.Numbers();

            using var session = GraphStoreHolder.Store.OpenSession();
        }
    }
}
