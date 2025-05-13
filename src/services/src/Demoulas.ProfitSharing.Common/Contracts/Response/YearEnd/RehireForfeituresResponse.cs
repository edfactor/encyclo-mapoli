
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record RehireForfeituresResponse
{
    public required int BadgeNumber { get; set; }
    public required string? FullName { get; set; }
    public short StoreNumber { get; set; }
    public required string Ssn { get; set; }
    public required DateOnly ReHiredDate { get; set; }
    public required DateOnly HireDate { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public decimal? NetBalanceLastYear { get; init; }
    public decimal? VestedBalanceLastYear { get; set; }
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
            StoreNumber = 61,
            NetBalanceLastYear = (decimal)1_234_567.89,
            VestedBalanceLastYear = 229991.667850168000000M,

            ReHiredDate = new DateTime(2024, 12, 01, 01, 01, 01, DateTimeKind.Local).ToDateOnly(),
            HireDate = new DateTime(2017, 10, 04, 01, 01, 01, DateTimeKind.Local).ToDateOnly(),
            TerminationDate = new DateTime(2021, 10, 04, 01, 01, 01, DateTimeKind.Local).ToDateOnly()
,
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
