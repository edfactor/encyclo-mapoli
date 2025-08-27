using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;

public sealed record ProfitSharingUnder21InactiveNoBalanceResponse
{
    public int BadgeNumber { get; set; }
    [MaskSensitive] public required string LastName { get; set; }
    [MaskSensitive] public required string FirstName { get; set; }
    public DateOnly BirthDate { get; set; }
    public DateOnly HireDate { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public byte Age { get; set; }
    public byte EnrollmentId { get; set; }
    public bool IsExecutive { get; set; }
    [MaskSensitive] public string FullName => $"{LastName}, {FirstName}";

    public const string REPORT_NAME = "Inactive/Terminated Under 21";
    public static ProfitSharingUnder21InactiveNoBalanceResponse SampleResponse()
    {
        return new ProfitSharingUnder21InactiveNoBalanceResponse()
        {
            BadgeNumber = 700312,
            LastName = "Methers",
            FirstName = "Patricia",
            BirthDate = new DateOnly(2007, 4, 29),
            HireDate = new DateOnly(2025, 5, 10),
            TerminationDate = new DateOnly(2025, 7, 11),
            Age = 19,
            EnrollmentId = 1,
        };
    }
}
