namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record ProfitSharingDistributionsByAge
{
    public required int Age { get; set; }
    public int EmployeeCount { get; set; }
    public decimal Amount { get; set; }
    public required string EmploymentType { get; set; }
    

    public static ProfitSharingDistributionsByAge ResponseExample()
    {
        return new ProfitSharingDistributionsByAge
        {
           Age = 32,
           EmploymentType = "Full Time",
           Amount = (decimal)159_451.46,
           EmployeeCount = 7
        };
    }
}
