namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

internal sealed record ContributionYears
{
    internal int BadgeNumber { get; init; }
    internal byte YearsInPlan { get; init; }
}
