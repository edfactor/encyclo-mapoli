using System.ComponentModel;
using System.Reflection;
using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record ExecutiveHoursAndDollarsRequest : ProfitYearRequest
{
    public long? Ssn { get; set; }
    public int? BadgeNumber { get; set; }
    public string? FullName { get; set; }
    
    public bool? HasExecutiveHoursAndDollars { get; set; }

}
