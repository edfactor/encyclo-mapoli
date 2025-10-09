using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;


public record ProfitSharingUnder21ReportDetail(
    short StoreNumber,
    int BadgeNumber,
    string FirstName,
    string LastName,
    string Ssn,
    byte ProfitSharingYears,
    bool IsNew,
    decimal ThisYearHours,
    decimal LastYearHours,
    DateOnly HireDate,
    DateOnly? FullTimeDate,
    DateOnly? TerminationDate,
    DateOnly DateOfBirth,
    short Age,
    char EmploymentStatusId,
    decimal CurrentBalance,
    byte EnrollmentId) : IIsExecutive
{
    public static ProfitSharingUnder21ReportDetail ResponseExample()
    {
        return new ProfitSharingUnder21ReportDetail(44, 700312, "Wendy", "Johnson", "xxx-xx-4002", 2, false, 1500, 1402, new DateOnly(2023, 4, 1), null, null, new DateOnly(1991, 9, 24), 20, 'a', 14002, 2);
    }

    public bool IsExecutive { get; set; }
};
