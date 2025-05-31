namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public sealed record MemberDetails
{
    public bool IsEmployee { get; init; }
    public int BadgeNumber { get; init; }
    public short PsnSuffix { get; init; }
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
    public byte? EnrollmentId { get; init; }
    public string? Enrollment { get; init; }
    public DateOnly? HireDate { get; init; }
    public DateOnly? TerminationDate { get; init; } = null;
    public DateOnly? ReHireDate { get; init; } = null;
    public short StoreNumber { get; set; }
    public decimal BeginPSAmount { get; set; }
    public decimal CurrentPSAmount { get; set; }
    public decimal BeginVestedAmount { get; set; }
    public decimal CurrentVestedAmount { get; set;}
    public decimal CurrentEtva { get; set; }
    public decimal PreviousEtva { get; set; }
    public List<int> Missives { get; set; } = new List<int>();
    public string? EmploymentStatus { get; set; }
}
