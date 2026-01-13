using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

public class ProfitDetail : ModifiedBase
{
    public static class Constants
    {
        public const byte ProfitYearIterationMilitary = 1;
        public const byte ProfitYearIterationClassActionFund = 2;
    }

    public int Id { get; set; }
    public int Ssn { get; set; }
    public short ProfitYear { get; set; }
    public byte ProfitYearIteration { get; set; }
    public int DistributionSequence { get; set; }
    public ProfitCode ProfitCode { get; set; } = null!;
    public required byte ProfitCodeId { get; set; }
    public decimal Contribution { get; set; }
    public decimal Earnings { get; set; }
    public decimal Forfeiture { get; set; }

    public byte MonthToDate { get; set; }
    public short YearToDate { get; set; }
    public string? Remark { get; set; }
    public byte? ZeroContributionReasonId { get; set; }
    public ZeroContributionReason? ZeroContributionReason { get; set; }
    public decimal FederalTaxes { get; set; }
    public decimal StateTaxes { get; set; }
    public TaxCode? TaxCode { get; set; }
    public char? TaxCodeId { get; set; }
    public byte? CommentTypeId { get; set; }
    public CommentType? CommentType { get; set; }
    public string? CommentRelatedCheckNumber { get; set; }
    public string? CommentRelatedState { get; set; }
    public long? CommentRelatedOracleHcmId { get; set; }
    public short? CommentRelatedPsnSuffix { get; set; }
    public bool? CommentIsPartialTransaction { get; set; }
    public sbyte YearsOfServiceCredit { get; set; }

    /// <summary>
    /// References the original ProfitDetail record that this record reverses.
    /// Used to prevent double-reversals of the same source record.
    /// A reversal chain is allowed (A → REV(A) → REV(REV(A))), but a single
    /// source record cannot have multiple direct reversals.
    /// </summary>
    public int? ReversedFromProfitDetailId { get; set; }

    /// <summary>
    /// Navigation property to the original ProfitDetail that was reversed.
    /// </summary>
    public ProfitDetail? ReversedFromProfitDetail { get; set; }
}
