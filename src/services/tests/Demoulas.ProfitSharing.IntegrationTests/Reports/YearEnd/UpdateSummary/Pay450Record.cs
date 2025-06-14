namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.UpdateSummary;

// Represents a row of the PAY450 report
public sealed record Pay450Record
{
    public string BadgeAndStore { get; init; } = "";
    public string Name { get; init; } = "";
    public decimal BeforeAmount { get; init; }
    public decimal BeforeVested { get; init; }
    public int? BeforeYears { get; init; }
    public int? BeforeEnroll { get; init; }
    public decimal AfterAmount { get; init; }
    public decimal AfterVested { get; init; }
    public int? AfterYears { get; init; }
    public int? AfterEnroll { get; init; }
}
