namespace Demoulas.ProfitSharing.Services.Reports.ProfitShareUpdate;

/// <summary>
///     Values provided externally used to guide how to award Earnings,Contributions and forfeiture points
/// </summary>
public record AdjustmentAmounts(
    decimal ContributionPercent,
    decimal IncomingForfeitPercent,
    decimal EarningsPercent,
    decimal SecondaryEarningsPercent,
    long MaxAllowedContributions,
    long BadgeToAdjust,
    long BadgeToAdjust2,
    decimal AdjustEarningsAmount,
    decimal AdjustEarningsSecondaryAmount,
    decimal AdjustIncomingForfeitAmount,
    decimal AdjustContributionAmount);
