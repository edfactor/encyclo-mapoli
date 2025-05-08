namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record ForfeitureAdjustmentReportResponse:ReportResponseBase<ForfeitureAdjustmentReportDetail>
{
    public static readonly string REPORT_NAME = "FORFEITURES REPORT";
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
            Response = new Demoulas.Common.Contracts.Contracts.Response.PaginatedResponseDto<ForfeitureAdjustmentReportDetail>()
            {
                Total = 1,
                Results = new List<ForfeitureAdjustmentReportDetail>() { ForfeitureAdjustmentReportDetail.ResponseExample() }
            }
        };
    }
}
