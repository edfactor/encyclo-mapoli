using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Services.Internal.ProfitShareUpdate;

/// <summary>
///     Newly computed (after applying the adjustments from the user) values for a member.
/// </summary>
internal sealed record MemberTotals
{
    public decimal PointsDollars { get; set; }
    public decimal NewCurrentAmount { get; set; }

    [MaskSensitive]
    public int EarnPoints { get; set; }
    public decimal IncomingForfeitureAmount { get; set; }
    public decimal ContributionAmount { get; set; }
    public decimal EarningsAmount { get; set; }
    public decimal SecondaryEarningsAmount { get; set; }
}
