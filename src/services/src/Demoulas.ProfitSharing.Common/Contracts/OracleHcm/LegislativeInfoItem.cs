using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public record LegislativeInfoItem(
[property: JsonPropertyName("Gender")] string? Gender,
[property: JsonPropertyName("MaritalStatus")] char? MaritalStatus,
[property: JsonPropertyName("LastUpdateDate")] DateTimeOffset LastUpdateDate
    );
