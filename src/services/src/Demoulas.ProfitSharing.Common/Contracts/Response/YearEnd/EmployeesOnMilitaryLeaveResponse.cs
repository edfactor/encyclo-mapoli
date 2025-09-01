
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record EmployeesOnMilitaryLeaveResponse: IIsExecutive
{
    public required byte DepartmentId { get; set; }
    public required int BadgeNumber { get; set; }
    public required string Ssn { get; set; }
    public required string? FullName { get; set; }
    public required DateOnly DateOfBirth { get; set; }
    public required DateOnly? TerminationDate { get; set; }
    public required bool IsExecutive { get; set; }


    public static EmployeesOnMilitaryLeaveResponse ResponseExample()
    {
        return new EmployeesOnMilitaryLeaveResponse
        {
            DepartmentId = 6,
            BadgeNumber = 123,
            Ssn = "XXX-XX-1234",
            FullName = "Doe, John",
            TerminationDate = DateTime.Today.ToDateOnly(),
            DateOfBirth = DateTime.Today.AddYears(-25).ToDateOnly(),
            IsExecutive = false,
        };
    }
}
