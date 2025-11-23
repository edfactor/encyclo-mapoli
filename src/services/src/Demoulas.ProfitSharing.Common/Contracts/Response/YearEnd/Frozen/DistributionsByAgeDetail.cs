using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
public sealed record DistributionsByAgeDetail
{
    [MaskSensitive] public required short Age { get; set; }
    public int EmployeeCount => BadgeNumbers.Count;
    public decimal Amount { get; set; }
    public decimal HardshipAmount { get; set; }
    public decimal RegularAmount { get; set; }
    public required string EmploymentType { get; set; }
    public int RegularEmployeeCount { get; set; }
    public int HardshipEmployeeCount { get; set; }
    public HashSet<int> BadgeNumbers { get; set; } = [];

    public static DistributionsByAgeDetail ResponseExample()
    {
        return new DistributionsByAgeDetail
        {
            Age = 32,
            EmploymentType = "Full Time",
            Amount = (decimal)159_451.46,
            BadgeNumbers =
            [
                123456,
                234567,
                345678,
                456789,
                567890
            ]
        };
    }
}
