
using Demoulas.Util.Extensions;
using System.Data.SqlTypes;
using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record MilitaryAndRehireReportResponse
{
    public required byte DepartmentId { get; set; }
    public required int BadgeNumber { get; set; }
    public required string Ssn { get; set; }
    public required string? FullName { get; set; }
    public required DateOnly DateOfBirth { get; set; }
    public required DateOnly? TerminationDate { get; set; }


    public static MilitaryAndRehireReportResponse ResponseExample()
    {
        return new MilitaryAndRehireReportResponse
        {
            DepartmentId = 6,
            BadgeNumber = 123,
            Ssn = "XXX-XX-1234",
            FullName = "Doe, John",
            TerminationDate = DateTime.Today.ToDateOnly(),
            DateOfBirth = DateTime.Today.AddYears(-25).ToDateOnly()
        };
    }
}
