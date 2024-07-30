using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
public record NationalIdentifiers(
    [property: JsonPropertyName("items")] IReadOnlyList<NationalIdentifier> Items,
    [property: JsonPropertyName("count")] int? Count,
    [property: JsonPropertyName("hasMore")] bool? HasMore,
    [property: JsonPropertyName("limit")] int? Limit,
    [property: JsonPropertyName("offset")] int? Offset
);
