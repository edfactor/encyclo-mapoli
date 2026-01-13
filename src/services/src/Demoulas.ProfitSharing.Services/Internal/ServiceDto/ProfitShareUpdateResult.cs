namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

public sealed class ProfitShareUpdateResult
{
    internal bool HasExceededMaximumContributions { get; set; }
    internal List<ProfitShareUpdateMember> Members { get; set; } = new List<ProfitShareUpdateMember>();
}
