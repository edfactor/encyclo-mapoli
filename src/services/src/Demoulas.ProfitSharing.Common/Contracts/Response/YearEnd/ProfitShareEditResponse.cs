
namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

/// <summary>
/// The ProfitShareUpdateTotals of all the YE Update Transactions (aka PROFIT_DETAIL rows)
/// </summary>
public sealed record ProfitShareEditResponse : ReportResponseBase<ProfitShareEditMemberRecordResponse>
{
    public required decimal BeginningBalanceTotal { get; set; }
    public required decimal ContributionGrandTotal { get; set; }
    public required decimal IncomingForfeitureGrandTotal { get; set; }
    public required decimal EarningsGrandTotal { get; set; }
}
