using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public enum SearchBy
{
    Ssn,
    EmployeeId
}
public sealed class BalanceEndpointRequest
{
    [DefaultValue(SearchBy.Ssn)]
    public required SearchBy SearchType { get; set; } //Ssn or EmployeeId
    public required string Id { get; set; }
    public short ProfitYear { get; set; }
    public StringComparison Comparison { get; set; }
}
