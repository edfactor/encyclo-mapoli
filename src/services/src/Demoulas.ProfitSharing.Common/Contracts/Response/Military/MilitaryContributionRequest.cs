namespace Demoulas.ProfitSharing.Common.Contracts.Request.Military
{
    public sealed record MilitaryContributionResponse : YearRequest
    {
        public int BadgeNumber { get; init; }
        public decimal Amount { get; init; }
        public DateOnly ContributionDate { get; init; }
    }
}
