using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public record BalanceItem(
    [property: JsonPropertyName("BalanceTypeId")] long BalanceTypeId,
    [property: JsonPropertyName("TotalValue1")] decimal TotalValue1,
    [property: JsonPropertyName("TotalValue2")] decimal TotalValue2,
    [property: JsonPropertyName("DefbalId1")] long DefbalId1,
    [property: JsonPropertyName("DimensionName")] string DimensionName
);
