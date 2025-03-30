namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public sealed record ProfitShareUpdateResponse : ReportResponseBase<ProfitShareUpdateMemberResponse>
{
    public required bool HasExceededMaximumContributions { get; set; }
    public required AdjustmentsSummaryDto AdjustmentsSummary { get; set; }
    public required TotalsDto Totals { get; set; }
}
