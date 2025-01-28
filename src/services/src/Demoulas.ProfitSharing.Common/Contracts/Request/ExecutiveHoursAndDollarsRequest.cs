namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record ExecutiveHoursAndDollarsRequest : ProfitYearRequest
{
    public int? BadgeNumber { get; set; }
    public string? FullNameContains { get; set; }
    public bool? HasExecutiveHoursAndDollars { get; set; }
    public string? Ssn { get; set; }
}
