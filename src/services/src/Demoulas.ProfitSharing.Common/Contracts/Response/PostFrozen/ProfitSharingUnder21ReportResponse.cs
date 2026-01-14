namespace Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;

public sealed record ProfitSharingUnder21ReportResponse : ReportResponseBase<ProfitSharingUnder21ReportDetail>
{
    public static readonly string REPORT_NAME = "PROFIT SHARING UNDER AGE 21 REPORT";

    public ProfitSharingUnder21ReportResponse()
    {
        ReportName = REPORT_NAME;
    }

    public required ProfitSharingUnder21TotalForStatus ActiveTotals { get; set; }
    public required ProfitSharingUnder21TotalForStatus InactiveTotals { get; set; }
    public required ProfitSharingUnder21TotalForStatus TerminatedTotals { get; set; }
    public int TotalUnder21 { get; set; }

    public static ProfitSharingUnder21ReportResponse ResponseExample()
    {
        return new ProfitSharingUnder21ReportResponse
        {
            ReportName = REPORT_NAME,
            ReportDate = DateTimeOffset.UtcNow,
            ActiveTotals = new ProfitSharingUnder21TotalForStatus(4, 1, 5),
            InactiveTotals = new ProfitSharingUnder21TotalForStatus(3, 9, 2),
            TerminatedTotals = new ProfitSharingUnder21TotalForStatus(6, 8, 7),
            TotalUnder21 = 45,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new Demoulas.Common.Contracts.Contracts.Response.PaginatedResponseDto<ProfitSharingUnder21ReportDetail>
            {
                Total = 1
            }
        };
    }
}
