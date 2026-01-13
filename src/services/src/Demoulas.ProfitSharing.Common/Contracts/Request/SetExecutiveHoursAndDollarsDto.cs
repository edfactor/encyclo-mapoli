using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record SetExecutiveHoursAndDollarsDto : IBadgeNumberRequest
{
    public int BadgeNumber { get; set; }
    public decimal ExecutiveHours { get; set; }
    public decimal ExecutiveDollars { get; set; }

    public static SetExecutiveHoursAndDollarsDto RequestExample()
    {
        return new() { BadgeNumber = 9999, ExecutiveDollars = 721, ExecutiveHours = 1001 };
    }
}
