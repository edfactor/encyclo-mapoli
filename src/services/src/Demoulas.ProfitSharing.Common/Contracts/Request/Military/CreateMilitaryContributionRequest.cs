namespace Demoulas.ProfitSharing.Common.Contracts.Request.Military
{
    public sealed record CreateMilitaryContributionRequest : YearRequest
    {
        public int BadgeNumber { get; init; }
        public decimal ContributionAmount { get; init; }
        public DateOnly ContributionDate { get; init; }
    }
}
