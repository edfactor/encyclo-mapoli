namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
///     Values provided by user to guide how to update Earnings,Contributions and forfeiture points are updated for the new PS year
/// </summary>
public sealed record ProfitShareUpdateRequest : ProfitYearRequest
{
    public decimal ContributionPercent { get; set; }
    public decimal IncomingForfeitPercent { get; set; }
    public decimal EarningsPercent { get; set; }
    public decimal? SecondaryEarningsPercent { get; set; }
    public long MaxAllowedContributions { get; set; }
    public long? BadgeToAdjust { get; set; }
    public long? BadgeToAdjust2 { get; set; }
    public decimal? AdjustContributionAmount { get; set; }
    public decimal? AdjustEarningsAmount { get; set; }
    public decimal? AdjustIncomingForfeitAmount { get; set; }
    public decimal? AdjustEarningsSecondaryAmount { get; set; }

    public static new ProfitShareUpdateRequest RequestExample()
    {
        return new ProfitShareUpdateRequest
        {
            ContributionPercent = 15,
            IncomingForfeitPercent = 4,
            EarningsPercent = 2,
            SecondaryEarningsPercent = 0,
            MaxAllowedContributions = 30_000,
            BadgeToAdjust = 7773838,
            BadgeToAdjust2 = 0,
            AdjustContributionAmount = 11,
            AdjustEarningsAmount = 12,
            AdjustIncomingForfeitAmount = 15,
            AdjustEarningsSecondaryAmount = 0
        };
    }
}
