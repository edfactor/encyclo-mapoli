using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;

public record Headers(
    [property: JsonPropertyName("ETag")] string ETag
);