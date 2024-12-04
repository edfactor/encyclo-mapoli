namespace Demoulas.ProfitSharing.Services.InternalDto;
internal sealed record ContributionYears
{
    public int BadgeNumber { get; init; }
    public byte YearsInPlan { get; init; }
}
