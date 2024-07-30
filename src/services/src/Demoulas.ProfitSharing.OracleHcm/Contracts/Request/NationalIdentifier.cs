using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;

public record NationalIdentifier(
    [property: JsonPropertyName("NationalIdentifierId")] long? NationalIdentifierId,
    [property: JsonPropertyName("NationalIdentifierNumber")] string NationalIdentifierNumber,
    [property: JsonPropertyName("LastUpdateDate")] DateTime? LastUpdateDate,
    [property: JsonPropertyName("PrimaryFlag")] bool? PrimaryFlag
);
