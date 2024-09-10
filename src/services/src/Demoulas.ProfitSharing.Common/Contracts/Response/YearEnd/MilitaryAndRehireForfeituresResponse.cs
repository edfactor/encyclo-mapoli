
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record MilitaryAndRehireForfeituresResponse
{
    public required int BadgeNumber { get; set; }
    public required string? FullName { get; set; }
    public required string Ssn { get; set; }
    public required DateOnly ReHiredDate { get; set; }
    public required byte CompanyContributionYears { get; set; }
    public required decimal HoursCurrentYear { get; set; }
    public required List<MilitaryRehireProfitSharingDetailResponse> Details { get; set; }


    public static MilitaryAndRehireForfeituresResponse ResponseExample()
    {
        return new MilitaryAndRehireForfeituresResponse
        {
            BadgeNumber = 123,
            Ssn = "XXX-XX-1234",
            FullName = "Doe, John",
            HoursCurrentYear = (decimal)2345.6,
            CompanyContributionYears = 3,
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
