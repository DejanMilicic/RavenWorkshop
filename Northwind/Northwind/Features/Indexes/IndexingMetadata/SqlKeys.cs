using Newtonsoft.Json;

namespace Northwind.Features.Indexes.IndexingMetadata;

public class SqlKeys
{
    [JsonProperty("id")]
    public string Id { get; set; }
}
