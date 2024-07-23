// OracleDemographics myDeserializedClass = JsonSerializer.Deserialize<OracleDemographics>(myJsonResponse);

using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;

public record Context(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("headers")] Headers Headers,
    [property: JsonPropertyName("links")] IReadOnlyList<Link> Links
);
