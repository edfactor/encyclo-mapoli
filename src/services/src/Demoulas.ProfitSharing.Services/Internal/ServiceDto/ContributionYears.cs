namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;
internal sealed record ContributionYears
{
    public int BadgeNumber { get; init; }
    public byte YearsInPlan { get; init; }
}
