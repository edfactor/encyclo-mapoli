namespace Demoulas.ProfitSharing.Api.Utilities;

public record BuildInfo
{
    public string? BuildNumber { get; init; }
    public short? BuildId { get; init; }
    public string? PlanName { get; init; }
    public string? PlanRepository { get; init; }
    public string? RevisionNumber { get; init; }
}
