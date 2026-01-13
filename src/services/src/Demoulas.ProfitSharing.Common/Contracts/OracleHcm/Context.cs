using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public record Context(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("headers")] Headers Headers,
    [property: JsonPropertyName("links")] IReadOnlyList<Link> Links
);
