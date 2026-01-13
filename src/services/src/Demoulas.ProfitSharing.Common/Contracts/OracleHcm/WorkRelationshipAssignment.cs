namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

/// <summary>
/// Represents an assignment within a work relationship in the Oracle HCM system.
/// </summary>
/// <param name="LocationCode">The code representing the location of the assignment.</param>
/// <param name="JobCode">The code representing the job associated with the assignment (now a 3-4 character string code).</param>
/// <param name="PositionCode">The code representing the position of the assignment.</param>
/// <param name="FullPartTime">Indicates whether the assignment is full-time or part-time.</param>
/// <param name="Frequency">The frequency of the assignment, such as weekly or monthly.</param>
public record WorkRelationshipAssignment(
    short? LocationCode,
    string? JobCode,
    string? PositionCode,
    string? DepartmentName,
    long? DepartmentId,
    string? FullPartTime,
    long? AssignmentId,
    string? AssignmentName,
    string? AssignmentNumber,
    string? AssignmentCategory,
    string? AssignmentCategoryMeaning,
    char? Frequency
)
{
    /// <summary>The code representing the location of the assignment.</summary>
    public short? LocationCode { get; init; } = LocationCode;

    /// <summary>The code representing the job associated with the assignment.</summary>
    public string? JobCode { get; init; } = JobCode;

    /// <summary>The code representing the position of the assignment.</summary>
    public string? PositionCode { get; init; } = PositionCode;
    public string? DepartmentName { get; init; } = DepartmentName;
    public long? DepartmentId { get; init; } = DepartmentId;
    public long? AssignmentId { get; init; } = AssignmentId;

    /// <summary>Indicates whether the assignment is full-time or part-time.</summary>
    public string? FullPartTime { get; init; } = FullPartTime;
    public string? AssignmentCategory { get; init; } = AssignmentCategory;

    /// <summary>The frequency of the assignment, such as weekly or monthly.</summary>
    public char? Frequency { get; init; } = Frequency;
}
