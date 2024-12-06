namespace Demoulas.ProfitSharing.Services.Reports.ProfitShareUpdate;

/// <summary>
///     Values provided by user to guide how to award Earnings,Contributions and forfeiture points are updated for the new PS year
/// </summary>
public record AdjustmentAmounts(
    decimal ContributionPercent,
    decimal IncomingForfeitPercent,
    decimal EarningsPercent,
    decimal SecondaryEarningsPercent,
    long MaxAllowedContributions,
    long BadgeToAdjust,
    long BadgeToAdjust2,
    decimal AdjustContributionAmount,
    decimal AdjustEarningsAmount,
    decimal AdjustIncomingForfeitAmount,
    decimal AdjustEarningsSecondaryAmount
    );
