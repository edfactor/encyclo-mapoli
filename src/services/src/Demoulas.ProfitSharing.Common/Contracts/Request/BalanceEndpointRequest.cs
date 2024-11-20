using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public enum SearchBy
{
    Ssn = 0,
    EmployeeId = 1
}
public sealed class BalanceEndpointRequest
{
    [DefaultValue(SearchBy.Ssn)]
    public required SearchBy SearchType { get; set; } //Ssn or EmployeeId
    public required string Id { get; set; }
    public short ProfitYear { get; set; }
    public StringComparison Comparison { get; set; }
}
