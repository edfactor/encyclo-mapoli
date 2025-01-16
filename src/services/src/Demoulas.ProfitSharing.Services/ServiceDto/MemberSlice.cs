namespace Demoulas.ProfitSharing.Services.ServiceDto;

/// <summary>
/// A slice of a member in the profit sharing system. An instance of this slice is either employee
/// information or beneficiary information.  People are grouped by their full name.
/// </summary>
public sealed record MemberSlice
{
    public int PsnSuffix { get; init; }
    public required int BadgeNumber { get; init; }
    public long Ssn { get; init; }
    public decimal HoursCurrentYear { get; init; }
    public char EmploymentStatusCode { get; init; }
    public string? FullName { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string? MiddleInitial { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public byte YearsInPs { get; init; }
    public DateOnly? BirthDate { get; init; }
    public DateOnly? TerminationDate { get; init; }
    public decimal IncomeRegAndExecCurrentYear { get; init; }
    public char? TerminationCode { get; init; }
    public byte? ZeroCont { get; init; }
    public byte EnrollmentId { get; init; }
    public decimal Etva { get; init; }
    public decimal BeneficiaryAllocation { get; init; }
}
