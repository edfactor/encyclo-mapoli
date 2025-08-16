using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record YearEndProfitSharingReportTotals
{
    [YearEndArchiveProperty]
    public decimal WagesTotal { get; set; }
    [YearEndArchiveProperty]
    public decimal HoursTotal { get; set; }
    [YearEndArchiveProperty]
    public decimal PointsTotal { get; set; }
    public decimal TerminatedWagesTotal { get; set; }
    public decimal TerminatedHoursTotal { get; set; }
    [YearEndArchiveProperty]
    public long NumberOfEmployees { get; set; }
    public int NumberOfNewEmployees { get; set; }
    public int NumberOfEmployeesUnder21 { get; set; }
    public long NumberOfEmployeesInPlan { get => NumberOfEmployees - NumberOfNewEmployees - NumberOfEmployeesUnder21; }
    public decimal TerminatedPointsTotal { get; set; }
    [YearEndArchiveProperty]
    public decimal BalanceTotal { get; set; }
    public decimal TerminatedBalanceTotal { get; set; }

    public static YearEndProfitSharingReportTotals SampleResponse()
    {
        return new YearEndProfitSharingReportTotals()
        {
            WagesTotal = 1000000.00m,
            HoursTotal = 200000.00m,
            PointsTotal = 5000.00m,
            TerminatedWagesTotal = 200000.00m,
            TerminatedHoursTotal = 40000.00m,
            NumberOfEmployees = 1500,
            NumberOfNewEmployees = 100,
            NumberOfEmployeesUnder21 = 300,
            TerminatedPointsTotal = 1000.00m,
            BalanceTotal = 250000.00m,
            TerminatedBalanceTotal = 50000.00m
        };
    }
}
