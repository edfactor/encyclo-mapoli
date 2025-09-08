using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public class EntryContent<TContext> where TContext : DeltaContextBase
{
    public List<TContext> Context { get; set; } = [];

    [JsonPropertyName("Changed Attributes")]
    public List<ChangedAttribute>? ChangedAttributes { get; set; } = [];
}
