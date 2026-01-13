using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.Job;

public sealed record UserSyncRequestDto : IBadgeNumberRequest
{
    public required int BadgeNumber { get; set; }
}
