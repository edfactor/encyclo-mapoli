using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public record MemberYearSummaryDto : IIsExecutive
{
    public required short StoreNumber { get; init; }
    public required int BadgeNumber { get; init; }
    [MaskSensitive] public required string FullName { get; init; } = string.Empty;

    public required string PayClassificationId { get; init; }
    public required string PayClassificationName { get; init; } = string.Empty;

    public decimal BeginningBalance { get; init; }
    public decimal Earnings { get; init; }
    public decimal Contributions { get; init; }
    public decimal Forfeitures { get; init; }
    public decimal Distributions { get; init; }
    public decimal EndingBalance { get; init; }
    public decimal VestedAmount { get; init; }
    public byte VestedPercent { get; init; }
    public DateOnly DateOfBirth { get; init; }
    public DateOnly HireDate { get; init; }
    public DateOnly? TerminationDate { get; init; }
    public byte? EnrollmentId { get; init; }
    public decimal ProfitShareHours { get; init; }
    [MaskSensitive] public string Street1 { get; set; } = string.Empty;
    [MaskSensitive] public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public int CertificateSort { get; set; }
    public bool IsExecutive { get; set; }
}

