namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record ExecutiveHoursAndDollarsRequest : ProfitYearRequest
{
    public int? BadgeNumber { get; set; }
#pragma warning disable DSM001
    public int? Ssn { get; set; }
#pragma warning restore DSM001
    public string? FullNameContains { get; set; }
    public bool? HasExecutiveHoursAndDollars { get; set; }
    public bool? IsMonthlyPayroll { get; set; }
}
