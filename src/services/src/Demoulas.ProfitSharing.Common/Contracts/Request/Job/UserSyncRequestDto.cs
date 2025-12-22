namespace Demoulas.ProfitSharing.Common.Contracts.Request.Job;

public sealed record UserSyncRequestDto
{
    public required int BadgeNumber { get; set; }
}
