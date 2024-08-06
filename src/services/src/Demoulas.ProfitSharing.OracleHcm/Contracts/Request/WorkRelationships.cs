using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
public record WorkRelationships(
    [property: JsonPropertyName("items")] IReadOnlyList<WorkRelationship> Items,
    [property: JsonPropertyName("totalResults")] int? TotalResults,
    [property: JsonPropertyName("count")] int? Count,
    [property: JsonPropertyName("hasMore")] bool? HasMore,
    [property: JsonPropertyName("limit")] int? Limit,
    [property: JsonPropertyName("offset")] int? Offset
);
