namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

/// <summary>
/// A member in the profit sharing system.   Can be an employee a beneficiary or both.
/// </summary>
internal sealed record Member
{
    public required int BadgeNumber { get; init; }
    public required short PsnSuffix { get; init; }
    internal int Ssn { get; init; }
    internal string? FullName { get; init; } = string.Empty;
    internal string FirstName { get; init; } = string.Empty;
    internal string LastName { get; init; } = string.Empty;
    internal DateOnly? Birthday { get; init; }
    internal decimal HoursCurrentYear { get; init; }
    internal decimal EarningsCurrentYear { get; init; }
    internal DateOnly? TerminationDate { get; init; }
    internal char? TerminationCode { get; init; }
    internal decimal DistributionAmount { get; init; }
    internal decimal BeginningAmount { get; set; }
    internal decimal ForfeitAmount { get; init; }
    internal decimal EndingBalance { get; init; }
    internal byte YearsInPlan { get; set; }
    internal decimal VestedBalance { get; init; }
    internal byte? ZeroCont { get; init; }
    internal byte EnrollmentId { get; init; }
    internal decimal Evta { get; init; }
    internal decimal BeneficiaryAllocation { get; init; }
    internal bool IsExecutive { get; init; }
    public short ProfitYear { get; set; }
}
