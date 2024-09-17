namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record WagesPreviousYearResponse
{
    public required int BadgeNumber { get; set; }
    public decimal IncomeLastYear { get; set; }
    public decimal HoursLastYear { get; set; }

    public static WagesPreviousYearResponse ResponseExample()
    {
        return new WagesPreviousYearResponse { BadgeNumber = 123456, HoursLastYear = 3265, IncomeLastYear = (decimal)25_325.18 };
    }

}
