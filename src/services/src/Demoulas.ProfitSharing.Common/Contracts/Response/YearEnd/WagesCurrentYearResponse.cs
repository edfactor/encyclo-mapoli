namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record WagesCurrentYearResponse
{
    public required int EmployeeId { get; set; }
    public decimal IncomeCurrentYear { get; set; }
    public decimal HoursCurrentYear { get; set; }

    public static WagesCurrentYearResponse ResponseExample()
    {
        return new WagesCurrentYearResponse { EmployeeId = 123456, HoursCurrentYear = 3265, IncomeCurrentYear = (decimal)25_325.18 };
    }
}
