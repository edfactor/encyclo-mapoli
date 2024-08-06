using System.Text.Json.Serialization;

namespace Demoulas.ProfitSharing.OracleHcm.Contracts.Request;

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
    public WorkRelationshipAssignment? Assignment => Assignments.Items.First();
}
