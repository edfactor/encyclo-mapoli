using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public sealed record GrossWagesReportResponse : ReportResponseBase<GrossWagesReportDetail>
{
    public static readonly string REPORT_NAME = "PROFIT SHARING GROSS WAGES";
    public GrossWagesReportResponse()
    {
        ReportName = REPORT_NAME;
        ReportDate = DateTimeOffset.Now;
    }

    public required decimal TotalGrossWages { get; set; }
    public required decimal TotalProfitSharingAmount { get; set; }
    public required decimal TotalLoans { get; set; }
    public required decimal TotalForfeitures { get; set; }

    public static GrossWagesReportResponse ResponseExample()
    {
        return new GrossWagesReportResponse()
        {
            ReportName = REPORT_NAME,
            ReportDate = DateTimeOffset.Now,
            TotalForfeitures = 0,
            TotalLoans = 22500,
            TotalGrossWages = 826962.97m,
            TotalProfitSharingAmount = 6272199.89m,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new Demoulas.Common.Contracts.Contracts.Response.PaginatedResponseDto<GrossWagesReportDetail>(new PaginationRequestDto())
            {
                Results = new List<GrossWagesReportDetail>() { GrossWagesReportDetail.ResponseExample() }
            }
        };
    }
}
