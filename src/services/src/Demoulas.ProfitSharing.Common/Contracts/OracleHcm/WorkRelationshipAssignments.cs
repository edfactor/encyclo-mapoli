using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public record WorkRelationshipAssignments(
    [property: JsonPropertyName("items")] IReadOnlyList<WorkRelationshipAssignment> Items,
    [property: JsonPropertyName("totalResults")]
    int? TotalResults,
    [property: JsonPropertyName("count")] int? Count,
    [property: JsonPropertyName("hasMore")]
    bool? HasMore,
    [property: JsonPropertyName("limit")] int? Limit,
    [property: JsonPropertyName("offset")] int? Offset
);

