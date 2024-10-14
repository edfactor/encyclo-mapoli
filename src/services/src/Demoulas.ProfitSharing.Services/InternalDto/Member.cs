namespace Demoulas.ProfitSharing.Services.InternalDto;

/// <summary>
/// A member in the profit sharing system.   Can be an employee a beneficiary or both.
/// </summary>
internal sealed record Member
{
    public required string Psn { get; init; }
    public string? FullName { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string? MiddleInitial { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateOnly? Birthday { get; init; }
    public decimal HoursCurrentYear { get; init; }
    public decimal EarningsCurrentYear { get; init; }
    public long Ssn { get; init; }
    public DateOnly? TerminationDate { get; init; }
    public char? TerminationCode { get; init; }
    public decimal DistributionAmount { get; init; }
    public decimal BeginningAmount { get; set; }
    public decimal CurrentVestedAmount { get; init; }
    public decimal ForfeitAmount { get; init; }
    public decimal EndingBalance { get; init; }
    public int YearsInPlan { get; set; }
    public decimal VestedBalance { get; init; }
    public byte? ZeroCont { get; init; }
    public byte EnrollmentId { get; init; }
    public decimal Evta { get; init; }
    public decimal BeneficiaryAllocation { get; init; }
}
