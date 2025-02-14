namespace Demoulas.ProfitSharing.Common.Contracts.Request.Military
{
    public sealed record MilitaryContributionRequest : ProfitYearRequest
    {
        public int BadgeNumber { get; init; }
    }
}
