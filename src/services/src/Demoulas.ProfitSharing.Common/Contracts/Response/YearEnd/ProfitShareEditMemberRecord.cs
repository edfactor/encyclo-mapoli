using Demoulas.ProfitSharing.Common.Extensions;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

/// <summary>
/// Represents a year end Transaction (aka row in PROFIT_DETAIL) as shown to the user
/// </summary>
public record ProfitShareEditMemberRecord
{
    public ProfitShareEditMemberRecord(ProfitShareUpdateMemberResponse mr, byte code)
    {
        IsEmployee = mr.IsEmployee;
        Ssn = mr.Ssn;
        Badge = mr.Badge;
        Psn = mr.Psn;
        Name = mr.Name;
        Code = code;
    }
    public bool IsEmployee { get; init; }
    public int Ssn { get; set; }
    public long Badge { get; set; }
    public long Psn { get; set; }
    public string? Name { get; set; }
    public byte Code { get; set; }
    public decimal ContributionAmount { get; set; }
    public decimal EarningAmount { get; set; }
    public decimal ForfeitureAmount { get; set; }
    public string? Remark { get; set; }
    public byte? CommentTypeId { get; set; }
    public string? RecordChangeSummary { get; set; }

    public byte ZeroContStatus { get; set; }
    public byte YearExtension { get; set; }
    
    public static ProfitShareEditMemberRecord ResponseExample()
    {
        return new ProfitShareEditMemberRecord(new ProfitShareUpdateMemberResponse(), 0)
        {
            Ssn = 777,
            Badge = 123,
            Psn = 0,
            Name = "VILLANUEVA, ISAAC",
            ContributionAmount = 4350m,
            EarningAmount = 50,
            ForfeitureAmount = 290,
            Remark = "V-ONLY",
            RecordChangeSummary = "18,19,20 > 1000"
        };
    }
}
