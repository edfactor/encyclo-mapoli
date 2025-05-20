using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record BalanceEndpointRequest
{
    [DefaultValue(SearchBy.Ssn)]
    public required SearchBy SearchType { get; set; } //Ssn or BadgeNumber
    public required string Id { get; set; }
    public short ProfitYear { get; set; }
}
