using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Shared;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;

public sealed record ProfitSharingUnder21InactiveNoBalanceResponse : IIsExecutive, INameParts
{
    public int BadgeNumber { get; set; }
    [MaskSensitive] public required string LastName { get; set; }
    [MaskSensitive] public required string FirstName { get; set; }
    [MaskSensitive] public string? MiddleName { get; set; }
    public DateOnly BirthDate { get; set; }
    public DateOnly HireDate { get; set; }
    public DateOnly? TerminationDate { get; set; }
    [MaskSensitive] public short Age { get; set; }
    public byte EnrollmentId { get; set; }
    public bool IsExecutive { get; set; }
    /// <summary>
    /// FullName is computed by the database from LastName, FirstName, and MiddleName.
    /// Format: "LastName, FirstName" or "LastName, FirstName M" (with middle initial if present)
    /// </summary>
    [MaskSensitive] public required string FullName { get; set; }

    public const string REPORT_NAME = "Inactive/Terminated Under 21";
    public static ProfitSharingUnder21InactiveNoBalanceResponse ResponseExample()
    {
        return new ProfitSharingUnder21InactiveNoBalanceResponse()
        {
            BadgeNumber = 700312,
            LastName = "Methers",
            FirstName = "Patricia",
            FullName = "Methers, Patricia",
            BirthDate = new DateOnly(2007, 4, 29),
            HireDate = new DateOnly(2025, 5, 10),
            TerminationDate = new DateOnly(2025, 7, 11),
            Age = 19,
            EnrollmentId = 1,
        };
    }
}
