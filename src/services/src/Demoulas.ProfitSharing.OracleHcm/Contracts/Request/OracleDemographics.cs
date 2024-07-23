using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;

public record OracleDemographics(
    [property: JsonPropertyName("items")] IReadOnlyList<OracleEmployee> Items,
    [property: JsonPropertyName("count")] int Count,
    [property: JsonPropertyName("hasMore")] bool HasMore,
    [property: JsonPropertyName("limit")] int Limit,
    [property: JsonPropertyName("offset")] int Offset,
    [property: JsonPropertyName("links")] IReadOnlyList<Link> Links
);
