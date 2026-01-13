using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public record EmailItem(
[property: JsonPropertyName("EmailAddressId")] string EmailAddressId,
[property: JsonPropertyName("EmailType")] string EmailType,
[property: JsonPropertyName("EmailAddress")] string EmailAddress,
[property: JsonPropertyName("PrimaryFlag")] bool? PrimaryFlag
    );
