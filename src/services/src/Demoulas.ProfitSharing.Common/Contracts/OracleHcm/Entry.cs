using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public class Entry<TContext> where TContext : DeltaContextBase
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Summary { get; set; }

    [JsonConverter(typeof(EntryContentConverterFactory))]
    public required EntryContent<TContext> Content { get; set; }

    public DateTime Updated { get; set; }
    public DateTime Published { get; set; }
    public List<DeltaLink> Links { get; set; } = [];
    public List<Author> Authors { get; set; } = [];
}
