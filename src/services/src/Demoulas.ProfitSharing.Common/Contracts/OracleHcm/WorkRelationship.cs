using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public record WorkRelationship(
    char WorkerType,
    DateOnly StartDate,
    bool OnMilitaryServiceFlag,
    DateOnly? TerminationDate,
    char? RevokeUserAccess,
    bool? PrimaryFlag,
    [property: JsonPropertyName("assignments")]
    WorkRelationshipAssignments Assignments
)
{
    public WorkRelationshipAssignment Assignment => Assignments.Items[0];
}
