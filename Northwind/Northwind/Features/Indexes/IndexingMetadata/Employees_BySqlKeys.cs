using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Northwind.Models.Entity;
using Raven.Client.Documents.Indexes;

namespace Northwind.Features.Indexes.IndexingMetadata;

public class Employees_BySqlKeys : AbstractIndexCreationTask<Employee>
{
    public class Entry
    {
        public string SqlKeyId { get; set; }
    }

    public Employees_BySqlKeys()
    {
        Map = employees => from employee in employees
            let value = JsonConvert.DeserializeObject<SqlKeys>(MetadataFor(employee).Value<string>("@sql-keys") ?? "")
            let id = value == null ? "" : value.Id.ToString()
            select new Entry
            {
                SqlKeyId = id
            };

        AdditionalAssemblies = new HashSet<AdditionalAssembly>
        {
            AdditionalAssembly.FromRuntime("Newtonsoft.Json", new HashSet<string> { "Newtonsoft.Json" })
        };
    }
}
