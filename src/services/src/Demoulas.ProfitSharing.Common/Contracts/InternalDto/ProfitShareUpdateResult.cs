namespace Demoulas.ProfitSharing.Common.Contracts.InternalDto;

public class ProfitShareUpdateResult
{
    public required bool HasExceededMaximumContributions { get; set; }
    public required List<ProfitShareUpdateMember> Members { get; set; }
}
