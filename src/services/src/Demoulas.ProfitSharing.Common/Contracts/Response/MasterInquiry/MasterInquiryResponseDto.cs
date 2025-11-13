using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;

public sealed record MasterInquiryResponseDto : IdRequest, IIsExecutive, IProfitYearRequest
{
    public bool IsEmployee => PsnSuffix == 0;
    public string Ssn { get; set; } = string.Empty;
    public short ProfitYear { get; set; }
    public byte ProfitYearIteration { get; set; }
    public int DistributionSequence { get; set; }
    public byte ProfitCodeId { get; set; }
    public decimal Contribution { get; set; }
    public decimal Earnings { get; set; }
    public decimal Forfeiture { get; set; }
    public byte MonthToDate { get; set; }
    public short YearToDate { get; set; }
    public string? Remark { get; set; }
    public byte? ZeroContributionReasonId { get; set; }
    public decimal FederalTaxes { get; set; }
    public decimal StateTaxes { get; set; }
    public char? TaxCodeId { get; set; }
    public int? CommentTypeId { get; set; }
    public string? CommentRelatedCheckNumber { get; set; }
    public string? CommentRelatedState { get; set; }
    public long? CommentRelatedOracleHcmId { get; set; }
    public short? CommentRelatedPsnSuffix { get; set; }
    public bool? CommentIsPartialTransaction { get; set; }
    public int? BadgeNumber { get; set; }
    public string? ProfitCodeName { get; set; }
    public string? ZeroContributionReasonName { get; set; }
    public string? TaxCodeName { get; set; }
    public string? CommentTypeName { get; set; }
    public short PsnSuffix { get; set; }
    public decimal? Payment { get; set; }
    public decimal? VestedBalance { get; set; }
    public decimal? VestingPercent { get; set; }
    public decimal? CurrentBalance { get; set; }
    public byte PayFrequencyId { get; set; }
    public DateTimeOffset TransactionDate { get; set; }
    public decimal CurrentIncomeYear { get; set; }
    public decimal CurrentHoursYear { get; set; }
    public char EmploymentStatusId { get; set; }
    public string? EmploymentStatus { get; set; }
    public bool IsExecutive { get; set; }
    public long? XFerQdroId { get; set; }
    [MaskSensitive]
    public string? XFerQdroName { get; set; }

    public static MasterInquiryResponseDto ResponseExample()
    {
        return new MasterInquiryResponseDto
        {
            Id = 12345,
            Ssn = "XXX-XX-2345",
            ProfitYear = 2023,
            ProfitYearIteration = 1,
            DistributionSequence = 4,
            ProfitCodeId = 2,
            Contribution = 2500.00m,
            Earnings = 175.50m,
            Forfeiture = 0.00m,
            MonthToDate = 6,
            YearToDate = 2023,
            Remark = "Annual profit distribution",
            ZeroContributionReasonId = null,
            FederalTaxes = 550.00m,
            StateTaxes = 125.75m,
            TaxCodeId = 'S',
            CommentTypeId = 3,
            CommentRelatedCheckNumber = "CHK123456",
            CommentRelatedState = "MA",
            CommentRelatedOracleHcmId = 987654321,
            CommentRelatedPsnSuffix = 1,
            CommentIsPartialTransaction = false,
            BadgeNumber = 54321,
            ProfitCodeName = "Standard Contribution",
            ZeroContributionReasonName = null,
            TaxCodeName = "Standard",
            CommentTypeName = "Information",
            PsnSuffix = 1,
            IsExecutive = false
        };
    }
}
