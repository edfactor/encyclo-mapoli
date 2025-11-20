using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Shared;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;


public record ProfitSharingUnder21ReportDetail : IIsExecutive, IFirstName, ILastName
{
    public bool IsExecutive { get; set; }
    public short StoreNumber { get; init; }
    public int BadgeNumber { get; init; }
    [MaskSensitive] public required string FirstName { get; init; }
    [MaskSensitive] public required string LastName { get; init; }
    public required string Ssn { get; init; }
    public byte ProfitSharingYears { get; init; }
    public bool IsNew { get; init; }
    public decimal ThisYearHours { get; init; }
    public decimal LastYearHours { get; init; }
    public DateOnly HireDate { get; init; }
    public DateOnly? FullTimeDate { get; init; }
    public DateOnly? TerminationDate { get; init; }

    [MaskSensitive]
    public DateOnly DateOfBirth { get; init; }
    public short Age { get; init; }
    public required string EmploymentStatusId { get; init; }
    public decimal CurrentBalance { get; init; }
    public required string EnrollmentId { get; init; }
}
