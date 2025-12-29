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
    [MaskSensitive] public required short Age { get; init; }
    public required char EmploymentStatusId { get; init; }
    public decimal CurrentBalance { get; init; }
    public required string EnrollmentId { get; init; }

    public static ProfitSharingUnder21ReportDetail ResponseExample()
    {
        return new ProfitSharingUnder21ReportDetail
        {
            IsExecutive = false,
            StoreNumber = 22,
            BadgeNumber = 700123,
            FirstName = "John",
            LastName = "Doe",
            Ssn = "xxx-xx-1234",
            ProfitSharingYears = 5,
            IsNew = false,
            ThisYearHours = 2080,
            LastYearHours = 2040,
            HireDate = new DateOnly(2019, 3, 15),
            FullTimeDate = new DateOnly(2019, 6, 1),
            TerminationDate = null,
            DateOfBirth = new DateOnly(2005, 8, 20),
            Age = 19,
            EmploymentStatusId = 'A',
            CurrentBalance = 15000.50m,
            EnrollmentId = "1"
        };
    }
}
