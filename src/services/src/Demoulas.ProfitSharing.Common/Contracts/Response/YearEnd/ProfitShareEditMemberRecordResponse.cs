
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

/// <summary>
/// Represents a year end Transaction (aka row in PROFIT_DETAIL) as shown to the user
/// </summary>
public record ProfitShareEditMemberRecordResponse : IIsExecutive
{
    public bool IsEmployee { get; init; }
    public int BadgeNumber { get; set; }
    public long Psn { get; set; }
    public string? Name { get; set; }
    public byte Code { get; set; }
    public decimal ContributionAmount { get; set; }
    public decimal EarningsAmount { get; set; }
    public decimal ForfeitureAmount { get; set; }
    public string? Remark { get; set; }
    public byte? CommentTypeId { get; set; }
    public string? RecordChangeSummary { get; set; }
    public byte? DisplayedZeroContStatus { get; set; }
    public byte YearExtension { get; set; }
    public required bool IsExecutive { get; set; }

    public static ProfitShareEditMemberRecordResponse ResponseExample()
    {
        return new ProfitShareEditMemberRecordResponse()
        {
            BadgeNumber = 123,
            Psn = 123,
            Name = "VILLANUEVA, ISAAC",
            ContributionAmount = 4350m,
            EarningsAmount = 50,
            ForfeitureAmount = 290,
            Remark = "V-ONLY",
            RecordChangeSummary = "18,19,20 > 1000",
            IsExecutive = false,
        };
    }
}
