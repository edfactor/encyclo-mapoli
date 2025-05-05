
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record RehireForfeituresResponse
{
    public required int BadgeNumber { get; set; }
    public required string? FullName { get; set; }
    public required string Ssn { get; set; }
    public required DateOnly ReHiredDate { get; set; }
    public required byte CompanyContributionYears { get; set; }
    public required decimal HoursCurrentYear { get; set; }
    public required byte EnrollmentId  { get; set; }
    public required string EnrollmentName { get; set; }
    public required IEnumerable<MilitaryRehireProfitSharingDetailResponse> Details { get; set; }
    public required string? EmploymentStatus { get; set; }


    public static RehireForfeituresResponse ResponseExample()
    {
        return new RehireForfeituresResponse
        {
            BadgeNumber = 123,
            Ssn = "XXX-XX-1234",
            FullName = "Doe, John",
            HoursCurrentYear = (decimal)2345.6,
            CompanyContributionYears = 3,
            EnrollmentId = 4,
            EnrollmentName = "New vesting plan has Forfeiture records",
            EmploymentStatus = "Terminated",
            ReHiredDate = DateTime.Today.AddYears(-2).ToDateOnly(),
            Details = new List<MilitaryRehireProfitSharingDetailResponse>
            {
                new MilitaryRehireProfitSharingDetailResponse
                {
                    Forfeiture = (decimal)3254.14,
                    ProfitYear = (short)DateTime.Today.AddYears(-3).Year,
                    Remark = "Example remarks here"
                }
            }
        };
    }
}
