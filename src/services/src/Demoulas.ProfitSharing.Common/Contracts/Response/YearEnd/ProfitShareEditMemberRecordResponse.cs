
namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

/// <summary>
/// Represents a year end Transaction (aka row in PROFIT_DETAIL) as shown to the user
/// </summary>
public record ProfitShareEditMemberRecordResponse
{
    public long Badge { get; set; }
    public long Psn { get; set; }
    public string? Name { get; set; }
    public short Code { get; set; }
    public decimal ContributionAmount { get; set; }
    public decimal EarningsAmount { get; set; }
    public decimal IncomingForfeitures { get; set; }
    public string? Reason { get; set; }
    
    public static ProfitShareEditMemberRecordResponse ResponseExample()
    {
        return new ProfitShareEditMemberRecordResponse
        {
            Badge = 123,
            Psn = 0,
            Name = "VILLANUEVA, ISAAC",
            Code = 0,
            ContributionAmount = 4350m,
            EarningsAmount = 50,
            IncomingForfeitures = 290,
            Reason = ">64 & >5 100%",
        };
    }

}
