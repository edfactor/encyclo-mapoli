namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

public class ProfitShareUpdateResult
{
    public required bool HasExceededMaximumContributions { get; set; }
    public required List<ProfitShareUpdateMember> Members { get; set; }
}
