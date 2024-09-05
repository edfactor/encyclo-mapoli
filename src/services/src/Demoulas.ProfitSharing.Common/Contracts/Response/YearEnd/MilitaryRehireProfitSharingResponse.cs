
using Demoulas.Util.Extensions;
using System.Data.SqlTypes;
using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record MilitaryRehireProfitSharingResponse
{
    public required int BadgeNumber { get; set; }
    public required string? FullName { get; set; }
    public required string Ssn { get; set; }
    public required DateOnly ReHiredDate { get; set; }
    public required byte Years { get; set; }
    public required decimal Hours { get; set; }
    public required List<MilitaryRehireProfitSharingDetailResponse> Details { get; set; }


    public static MilitaryRehireProfitSharingResponse ResponseExample()
    {
        return new MilitaryRehireProfitSharingResponse
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
