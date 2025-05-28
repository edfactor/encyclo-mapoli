namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record ForfeitureAdjustmentReportResponse:ReportResponseBase<ForfeitureAdjustmentReportDetail>
{
    public static readonly string REPORT_NAME = "Forfeiture Adjustments";
    public ForfeitureAdjustmentReportResponse()
    {
        this.ReportName = REPORT_NAME;
    }
    public int TotatNetBalance { get; set; }
    public int TotatNetVested { get; set; }

    public static ForfeitureAdjustmentReportResponse ResponseExample()
    {
        return new ForfeitureAdjustmentReportResponse()
        {
            ReportName = REPORT_NAME,
            ReportDate = DateTime.UtcNow,
            TotatNetBalance = 1000,
            TotatNetVested = 1000,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new Demoulas.Common.Contracts.Contracts.Response.PaginatedResponseDto<ForfeitureAdjustmentReportDetail>()
            {
                Total = 1,
                Results = new List<ForfeitureAdjustmentReportDetail>() { ForfeitureAdjustmentReportDetail.ResponseExample() }
            }
        };
    }
}
