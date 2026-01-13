using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public record PayrollItem(
    [property: JsonPropertyName("PayrollActionId")] int PayrollActionId,
    [property: JsonPropertyName("ObjectActionId")] int ObjectActionId,
    [property: JsonPropertyName("PersonId")] long PersonId,
    [property: JsonPropertyName("SubmissionDate")] DateOnly? SubmissionDate
);
