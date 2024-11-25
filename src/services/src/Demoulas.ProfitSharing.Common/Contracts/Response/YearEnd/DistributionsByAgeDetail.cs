namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record DistributionsByAgeDetail
{
    public required int Age { get; set; }
    public int EmployeeCount { get; set; }
    public decimal Amount { get; set; }
    public decimal HardshipAmount { get; set; }
    public decimal RegularAmount { get; set; }
    public required string EmploymentType { get; set; }

    public static DistributionsByAgeDetail ResponseExample()
    {
        return new DistributionsByAgeDetail
        {
           Age = 32,
           EmploymentType = "Full Time",
           Amount = (decimal)159_451.46,
           EmployeeCount = 7
        };
    }
}
