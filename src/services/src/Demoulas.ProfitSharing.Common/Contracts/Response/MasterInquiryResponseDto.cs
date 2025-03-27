namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public sealed class MasterInquiryResponseDto
{
    public int Id { get; set; }
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
    public decimal FederalTaxes {get; set;}
    public decimal StateTaxes { get; set;}
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

    public static MasterInquiryResponseDto ResponseExample()
    {
        return new MasterInquiryResponseDto
        {
            Ssn = "XXX-XX-2345"
        };
    }
}
