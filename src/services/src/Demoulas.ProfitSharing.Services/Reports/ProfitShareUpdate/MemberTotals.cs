namespace Demoulas.ProfitSharing.Services.Reports.ProfitShareUpdate;

/// <summary>
///     Newly computed (after applying the adjustments from the user) values for a member.
/// </summary>
public class MemberTotals
{
    public decimal PointsDollars { get; set; }
    public decimal NewCurrentAmount { get; set; }
    public long EarnPoints { get; set; }
    public decimal IncomingForfeitureAmount { get; set; }
    public decimal ContributionAmount { get; set; }
    public decimal EarningsAmount { get; set; }
    public decimal SecondaryEarningsAmount { get; set; }
}
