using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
public sealed record ContributionsByAgeDetail
{
    [MaskSensitive] public required short Age { get; set; }
    public required int EmployeeCount { get; init; }
    public required decimal Amount { get; init; }

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
