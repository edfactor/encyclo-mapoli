using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public record NationalIdentifier(
    [property: JsonPropertyName("NationalIdentifierId")] long? NationalIdentifierId,
    [property: JsonPropertyName("NationalIdentifierNumber")] string NationalIdentifierNumber,
    [property: JsonPropertyName("LastUpdateDate")] DateTimeOffset? LastUpdateDate,
    [property: JsonPropertyName("PrimaryFlag")] bool? PrimaryFlag
);
