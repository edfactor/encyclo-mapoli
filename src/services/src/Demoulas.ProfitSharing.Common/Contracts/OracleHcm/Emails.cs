using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public record Emails(
    [property: JsonPropertyName("items")] IReadOnlyList<EmailItem> Items,
    [property: JsonPropertyName("totalResults")] int? TotalResults,
    [property: JsonPropertyName("count")] int? Count,
    [property: JsonPropertyName("hasMore")] bool? HasMore,
    [property: JsonPropertyName("limit")] int? Limit,
    [property: JsonPropertyName("offset")] int? Offset,
    [property: JsonPropertyName("links")] IReadOnlyList<Link> Links
);
