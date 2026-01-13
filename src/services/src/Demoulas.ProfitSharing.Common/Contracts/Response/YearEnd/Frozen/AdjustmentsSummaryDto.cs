namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

/// <summary>
///     Used to collect data about adjustments made to employees.  This data is displayed to the user to show the effect of thier inputs.
/// </summary>
public sealed record AdjustmentsSummaryDto
{
    public decimal IncomingForfeitureAmountUnadjusted { get; set; }
    public decimal IncomingForfeitureAmountAdjusted { get; set; }
    public decimal EarningsAmountUnadjusted { get; set; }
    public decimal EarningsAmountAdjusted { get; set; }
    public decimal SecondaryEarningsAmountUnadjusted { get; set; }
    public decimal SecondaryEarningsAmountAdjusted { get; set; }
    public decimal ContributionAmountUnadjusted { get; set; }
    public decimal ContributionAmountAdjusted { get; set; }

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static AdjustmentsSummaryDto ResponseExample()
    {
        return new AdjustmentsSummaryDto
        {
            IncomingForfeitureAmountUnadjusted = 50000.00m,
            IncomingForfeitureAmountAdjusted = 52000.00m,
            EarningsAmountUnadjusted = 150000.00m,
            EarningsAmountAdjusted = 155000.00m,
            SecondaryEarningsAmountUnadjusted = 75000.00m,
            SecondaryEarningsAmountAdjusted = 78000.00m,
            ContributionAmountUnadjusted = 250000.00m,
            ContributionAmountAdjusted = 260000.00m
        };
    }
}
