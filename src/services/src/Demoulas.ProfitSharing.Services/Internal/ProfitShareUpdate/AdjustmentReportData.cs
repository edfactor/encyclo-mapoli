namespace Demoulas.ProfitSharing.Services.Internal.ProfitShareUpdate;

/// <summary>
///     Used to collect data about adjustments made to employees.  This data is later used in the Adjustment Report.
/// </summary>
internal sealed record AdjustmentReportData
{
    public decimal IncomingForfeitureAmountUnadjusted { get; set; }
    public decimal IncomingForfeitureAmountAdjusted { get; set; }
    public decimal EarningsAmountUnadjusted { get; set; }
    public decimal EarningsAmountAdjusted { get; set; }
    public decimal SecondaryEarningsAmountUnadjusted { get; set; }
    public decimal SecondaryEarningsAmountAdjusted { get; set; }
    public decimal ContributionAmountUnadjusted { get; set; }
    public decimal ContributionAmountAdjusted { get; set; }
}
