using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;

public record OracleEmployee(
    [property: JsonPropertyName("PersonId")] long PersonId,
    [property: JsonPropertyName("PersonNumber")] int BadgeNumber,
    [property: JsonPropertyName("DateOfBirth")] DateOnly DateOfBirth,
    [property: JsonPropertyName("DateOfDeath")] DateOnly? DateOfDeath,
    [property: JsonPropertyName("CreatedBy")] string CreatedBy,
    [property: JsonPropertyName("CreationDate")] DateTimeOffset CreationDate,
    [property: JsonPropertyName("LastUpdatedBy")] string LastUpdatedBy,
    [property: JsonPropertyName("LastUpdateDate")] DateTimeOffset? LastUpdateDate,
    [property: JsonPropertyName("@context")] Context Context
);
