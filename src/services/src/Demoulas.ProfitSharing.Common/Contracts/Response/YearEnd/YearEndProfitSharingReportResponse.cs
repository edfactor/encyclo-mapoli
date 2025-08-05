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
    public decimal TerminatedWagesTotal { get; set; }
    public decimal TerminatedHoursTotal { get; set; }

    [YearEndArchiveProperty]
    public long NumberOfEmployees { get; set; }
    public int NumberOfNewEmployees { get; set; }
    public int NumberOfEmployeesUnder21 { get; set; }
    public long NumberOfEmployeesInPlan { get; set; }
    public decimal TerminatedPointsTotal { get; set; }
    
    [YearEndArchiveProperty]
    public decimal BalanceTotal { get; set; }
    public decimal TerminatedBalanceTotal { get; set; }

    public static YearEndProfitSharingReportResponse ResponseExample()
    {
        return new YearEndProfitSharingReportResponse
        {
            ReportName = "yearend-profit-sharing-report",
            ReportDate = DateTimeOffset.UtcNow,
            WagesTotal = 291941.55m,
            HoursTotal = 10052,
            PointsTotal = 2919m,
            TerminatedWagesTotal = 2002.51m,
            TerminatedHoursTotal = 205,
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
