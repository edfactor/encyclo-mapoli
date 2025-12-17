using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Services.MasterInquiry;

/// <summary>
/// Internal DTO representing a master inquiry item with member demographics and profit details.
/// Shared between Employee and Beneficiary inquiry services.
/// </summary>
public sealed class MasterInquiryItem
{
    public required ProfitDetail? ProfitDetail { get; init; }
    public required InquiryDemographics Member { get; init; }
    public required ProfitCode? ProfitCode { get; init; }
    public required ZeroContributionReason? ZeroContributionReason { get; init; }
    public required TaxCode? TaxCode { get; init; }
    public required CommentType? CommentType { get; init; }
    public DateTimeOffset TransactionDate { get; init; }
}

/// <summary>
/// Internal DTO for demographic data used in master inquiry operations.
/// Simplified representation shared across employee and beneficiary lookups.
/// </summary>
public sealed class InquiryDemographics
{
    public int BadgeNumber { get; init; }
    public required string FullName { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public byte PayFrequencyId { get; init; }
    public short PsnSuffix { get; init; }
    public int Ssn { get; init; }
    public decimal CurrentIncomeYear { get; init; }
    public decimal CurrentHoursYear { get; init; }
    public int Id { get; set; }
    public bool IsExecutive { get; set; }
    public char? EmploymentStatusId { get; init; }
}

/// <summary>
/// Internal DTO for SQL-translatable projection of master inquiry raw data.
/// Used for efficient database queries before final formatting.
/// </summary>
public sealed class MasterInquiryRawDto
{
    public int Id { get; init; }
    public int Ssn { get; init; }
    public short ProfitYear { get; init; }
    public byte ProfitYearIteration { get; init; }
    public int DistributionSequence { get; init; }
    public byte ProfitCodeId { get; init; }
    public string ProfitCodeName { get; init; } = string.Empty;
    public decimal Contribution { get; init; }
    public decimal Earnings { get; init; }
    public decimal Forfeiture { get; init; }
    public byte MonthToDate { get; init; }
    public short YearToDate { get; init; }
    public string? Remark { get; init; }
    public byte? ZeroContributionReasonId { get; init; }
    public string? ZeroContributionReasonName { get; init; }
    public decimal FederalTaxes { get; init; }
    public decimal StateTaxes { get; init; }
    public char? TaxCodeId { get; init; }
    public string? TaxCodeName { get; init; }
    public int? CommentTypeId { get; init; }
    public string? CommentTypeName { get; init; }
    public string? CommentRelatedCheckNumber { get; init; }
    public string? CommentRelatedState { get; init; }
    public long? CommentRelatedOracleHcmId { get; init; }
    public short? CommentRelatedPsnSuffix { get; init; }
    public bool? CommentIsPartialTransaction { get; init; }
    public int BadgeNumber { get; init; }
    public short PsnSuffix { get; init; }
    public byte PayFrequencyId { get; init; }
    public DateTimeOffset TransactionDate { get; init; }
    public decimal CurrentIncomeYear { get; init; }
    public decimal CurrentHoursYear { get; init; }
    public decimal Payment { get; set; }
    public bool IsExecutive { get; set; }
    public char? EmploymentStatusId { get; init; }
}
