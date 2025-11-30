namespace Demoulas.ProfitSharing.IntegrationTests.Reports.Termination;

/// <summary>
///     Represents a single data row from the QPAY066 Termination - Profit Sharing report.
/// </summary>
public sealed record QPay066Record
{
    public required int BadgeNumber { get; init; }
    public required short PsnSuffix { get; init; }
    public required string EmployeeName { get; init; }
    public required decimal BeginningBalance { get; init; }
    public required decimal BeneficiaryAllocation { get; init; }
    public required decimal DistributionAmount { get; init; }
    public required decimal Forfeit { get; init; }
    public required decimal EndingBalance { get; init; }
    public required decimal VestedBalance { get; init; }
    public DateOnly? DateTerm { get; init; }
    public required decimal YtdVstPsHours { get; init; }
    public required decimal VestedPercent { get; init; }
    public int? Age { get; init; }
    public char? EnrollmentCode { get; init; }
}
