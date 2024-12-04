namespace Demoulas.ProfitSharing.Services.InternalDto;
internal sealed record ContributionYears
{
    public int EmployeeId { get; init; }
    public byte YearsInPlan { get; init; }
}
