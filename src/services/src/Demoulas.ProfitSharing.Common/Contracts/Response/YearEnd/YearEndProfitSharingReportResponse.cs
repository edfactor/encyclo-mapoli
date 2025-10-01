using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record YearEndProfitSharingReportResponse : ReportResponseBase<YearEndProfitSharingReportDetail>
{
    [YearEndArchiveProperty]
    public decimal WagesTotal { get; set; }

    [YearEndArchiveProperty]
    public decimal HoursTotal { get; set; }
    public decimal PointsTotal { get; set; }

    [YearEndArchiveProperty]
    public long NumberOfEmployees { get; set; }
    public int NumberOfNewEmployees { get; set; }
    public int NumberOfEmployeesUnder21 { get; set; }
    public long NumberOfEmployeesInPlan { get; set; }
    
    [YearEndArchiveProperty]
    public decimal BalanceTotal { get; set; }

    public static YearEndProfitSharingReportResponse ResponseExample()
    {
        return new YearEndProfitSharingReportResponse
        {
            ReportName = "yearend-profit-sharing-report",
            ReportDate = DateTimeOffset.UtcNow,
            WagesTotal = 291941.55m,
            HoursTotal = 10052,
            PointsTotal = 2919m,
            NumberOfEmployees = 2051,
            NumberOfNewEmployees = 55,
            NumberOfEmployeesInPlan = 2044,
            NumberOfEmployeesUnder21 = 015,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new PaginatedResponseDto<YearEndProfitSharingReportDetail>(new PaginationRequestDto())
                { Total = 1, Results = new List<YearEndProfitSharingReportDetail>() { YearEndProfitSharingReportDetail.ResponseExample() } }
                
        };
    }
}
