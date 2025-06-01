namespace Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
public sealed record MemberProfitPlanDetails : MemberDetails
{
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
}
