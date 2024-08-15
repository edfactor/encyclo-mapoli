using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public record Headers(
    [property: JsonPropertyName("ETag")] string ETag
);
