namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public sealed record EmployeeDetails
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;

    public string Address { get; init; } = string.Empty;
    public string AddressCity { get; init; } = string.Empty;
    public string AddressState { get; init; } = string.Empty;
    public string AddressZipCode { get; init; } = string.Empty;
    public DateOnly DateOfBirth { get; init; }
    public string Ssn { get; init; } = string.Empty;
    public decimal YearToDateProfitSharingHours { get; init; }
    public int YearsInPlan { get; init; }
    public decimal PercentageVested { get; init; }
    public bool ContributionsLastYear { get; init; }
    public bool Enrolled { get; init; }
    public string BadgeNumber { get; init; } = string.Empty;
    public DateOnly HireDate { get; init; }
    public DateOnly? TerminationDate { get; init; } = null;
    public DateOnly? ReHireDate { get; init; } = null;
    public short StoreNumber { get; set; }
    public long BeginPSAmount { get; set; }
    public long CurrentPSAmount { get; set; }
    public long BeginVestedAmount { get; set; }
    public long CurrentVestedAmount { get; set;}
}
