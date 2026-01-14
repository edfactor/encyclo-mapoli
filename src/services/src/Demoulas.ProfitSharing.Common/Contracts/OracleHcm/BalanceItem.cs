using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

/// <summary>
/// Represents a single balance item returned from the Oracle HCM BalanceView endpoint.
/// Numeric values are delivered as JSON strings and deserialized via AllowReadingFromString.
/// Only the fields required by the application are explicitly modeled.
/// </summary>
[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public record BalanceItem(
    [property: JsonPropertyName("BalanceTypeId")] long BalanceTypeId,
    [property: JsonPropertyName("TotalValue1")] decimal TotalValue1,
    [property: JsonPropertyName("TotalValue2")] decimal TotalValue2,
    [property: JsonPropertyName("DefbalId1")] long DefbalId1,
    [property: JsonPropertyName("DimensionName")] string? DimensionName
);
