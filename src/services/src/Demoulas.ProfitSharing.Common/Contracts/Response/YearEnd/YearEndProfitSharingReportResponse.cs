using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record YearEndProfitSharingReportResponse : ReportResponseBase<YearEndProfitSharingReportDetail>
{
    public decimal WagesTotal { get; set; }
    public decimal HoursTotal { get; set; }
    public decimal PointsTotal { get; set; }
    public decimal TerminatedWagesTotal { get; set; }
    public decimal TerminatedHoursTotal { get; set; }
    public int NumberOfEmployees { get; set; }
    public int NumberOfNewEmployees { get; set; }
    public int NumberOfEmployeesUnder21 { get; set; }
    public int NumberOfEmployeesInPlan { get; set; }

    public static YearEndProfitSharingReportResponse ResponseExample()
    {
        return new YearEndProfitSharingReportResponse()
        {
            ReportName = "yearend-profit-sharing-report",
            ReportDate = DateTime.UtcNow,
            WagesTotal = 291941.55m,
            HoursTotal = 10052,
            PointsTotal = 2919m,
            TerminatedWagesTotal = 2002.51m,
            TerminatedHoursTotal = 205,
            NumberOfEmployees = 2051,
            NumberOfNewEmployees = 55,
            NumberOfEmployeesInPlan = 2044,
            NumberOfEmployeesUnder21 = 015,
            Response = new PaginatedResponseDto<YearEndProfitSharingReportDetail>(new PaginationRequestDto())
                { Total = 1, Results = new List<YearEndProfitSharingReportDetail>() { YearEndProfitSharingReportDetail.ResponseExample() } }
                
        };
    }
}

public sealed record YearEndProfitSharingReportDetail
{
    public required int BadgeNumber { get; set; }
    public required string EmployeeName { get; set; }
    public required short StoreNumber { get; set; }
    public required char EmployeeTypeCode { get; set; }
    public required string EmployeeTypeName { get; set; }
    public required DateOnly DateOfBirth { get; set; }
    public required byte Age { get; set; }
    public required string Ssn { get; set; }
    public required decimal Wages { get; set; }
    public required decimal Hours { get; set; }
    public short? Points { get; set; }
    public required bool IsUnder21 { get; set; }
    public required bool IsNew { get; set; }
    public char? EmployeeStatus { get; set; }
    public required decimal Balance { get; set; }
    public required short YearsInPlan { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public static YearEndProfitSharingReportDetail ResponseExample()
    {
        return new YearEndProfitSharingReportDetail()
        {
            BadgeNumber = 135,
            EmployeeName = "John Doe",
            StoreNumber = 23,
            EmployeeTypeCode = 'p',
            EmployeeTypeName = "Part Time",
            DateOfBirth = new DateOnly(1996, 9, 21),
            Age = 28,
            Ssn = "XXX-XX-1234",
            Wages = 26527,
            Hours = 1475,
            Points = 2653,
            IsUnder21 = false,
            IsNew = false,
            EmployeeStatus = ' ',
            Balance = 51351.55m,
            YearsInPlan = 1,
            FirstName = "John",
            LastName = "Doe"
        };
    }
}

