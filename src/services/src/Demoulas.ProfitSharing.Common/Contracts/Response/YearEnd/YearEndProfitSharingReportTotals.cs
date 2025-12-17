using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

[NoMemberDataExposed]
public sealed record YearEndProfitSharingReportTotals
{
    [YearEndArchiveProperty]
    public decimal WagesTotal { get; set; }
    [YearEndArchiveProperty]
    public decimal HoursTotal { get; set; }
    [YearEndArchiveProperty]
    public decimal PointsTotal { get; set; }
    [YearEndArchiveProperty]
    public long NumberOfEmployees { get; set; }
    public int NumberOfNewEmployees { get; set; }
    public int NumberOfEmployeesUnder21 { get; set; }
    public long NumberOfEmployeesInPlan { get => NumberOfEmployees - NumberOfNewEmployees - NumberOfEmployeesUnder21; }
    [YearEndArchiveProperty]
    public decimal BalanceTotal { get; set; }

    public static YearEndProfitSharingReportTotals SampleResponse()
    {
        return new YearEndProfitSharingReportTotals()
        {
            WagesTotal = 1000000.00m,
            HoursTotal = 200000.00m,
            PointsTotal = 5000.00m,
            NumberOfEmployees = 1500,
            NumberOfNewEmployees = 100,
            NumberOfEmployeesUnder21 = 300,
            BalanceTotal = 250000.00m
        };
    }
}
