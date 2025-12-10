using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public record Phones(
    [property: JsonPropertyName("items")] IReadOnlyList<PhoneItem> Items,
    [property: JsonPropertyName("totalResults")] int? TotalResults,
    [property: JsonPropertyName("count")] int? Count,
    [property: JsonPropertyName("hasMore")] bool? HasMore,
    [property: JsonPropertyName("limit")] int? Limit,
    [property: JsonPropertyName("offset")] int? Offset,
    [property: JsonPropertyName("links")] IReadOnlyList<Link> Links
);
