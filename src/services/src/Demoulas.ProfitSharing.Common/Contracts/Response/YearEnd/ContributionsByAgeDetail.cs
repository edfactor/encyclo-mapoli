namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record ContributionsByAgeDetail
{
    public required int Age { get; set; }
    public int EmployeeCount { get; set; }
    public decimal Amount { get; set; }

    public static ContributionsByAgeDetail ResponseExample()
    {
        return new ContributionsByAgeDetail
        {
           Age = 32,
           Amount = (decimal)159_451.46,
           EmployeeCount = 7
        };
    }
}
