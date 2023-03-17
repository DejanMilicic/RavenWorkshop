using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Northwind.Features.BulkInsert.RawJson
{
    public class Portfolio
    {
        private static Type t = typeof(Portfolio);

        private static string collectionName =
            Raven.Client.Documents.Conventions.DocumentConventions.DefaultGetCollectionName(t);

        private static string clrType = $"{t.FullName}, {t.Assembly.GetName().Name}";

        [IgnoreDataMember]
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string Name { get; set; }

        public string Owner { get; set; }

        public List<Entry> Entries = new List<Entry>();

        [DataMember(Name = "@metadata")]
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>
        {
            ["@collection"] = collectionName,
            ["Raven-Clr-Type"] = clrType
        };

        public class Entry
        {
            public string Symbol { get; set; }

            public decimal Price { get; set; }

            public int Quantity { get; set; }

            public int Factor1 { get; set; }

            public int Factor2 { get; set; }

            public int Factor3 { get; set; }
        }
    }
}
