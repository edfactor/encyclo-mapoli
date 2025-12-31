namespace Demoulas.ProfitSharing.Common.Contracts.Request.Military;

public sealed record CreateMilitaryContributionRequest : YearRequest
{
    public int BadgeNumber { get; init; }
    public decimal ContributionAmount { get; init; }
    public bool IsSupplementalContribution { get; init; }
    public DateTime ContributionDate { get; init; } = DateTime.Now;

    public static new CreateMilitaryContributionRequest RequestExample()
    {
        return new CreateMilitaryContributionRequest
        {
            BadgeNumber = 1234567,
            ContributionAmount = (decimal)1234.56,
            ProfitYear = (short)DateTime.Today.Year
        };
    }
}
