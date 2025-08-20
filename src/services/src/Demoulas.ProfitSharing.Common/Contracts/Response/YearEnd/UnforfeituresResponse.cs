
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

[YearEndArchiveProperty]
public sealed record UnforfeituresResponse
{
    public required int BadgeNumber { get; set; }
    [MaskSensitive] public required string? FullName { get; set; }
    public required string Ssn { get; set; }
    public required DateOnly ReHiredDate { get; set; }
    public required DateOnly HireDate { get; set; }
    public decimal? NetBalanceLastYear { get; init; }
    public decimal? VestedBalanceLastYear { get; set; }
    public required byte CompanyContributionYears { get; set; }
    public required short StoreNumber { get; set; }
    public required byte? EnrollmentId { get; set; }
    public required string EnrollmentName { get; set; }
    public required decimal WagesProfitYear { get; set; }
    public required decimal HoursProfitYear { get; set; }
    public required IEnumerable<RehireTransactionDetailResponse> Details { get; set; }


    public static UnforfeituresResponse ResponseExample()
    {
        return new UnforfeituresResponse
        {
            BadgeNumber = 123,
            Ssn = "XXX-XX-1234",
            FullName = "Doe, John",
            StoreNumber = 61,
            CompanyContributionYears = 3,
            ReHiredDate = new DateTime(2024, 12, 01, 01, 01, 01, DateTimeKind.Local).ToDateOnly(),
            HireDate = new DateTime(2017, 10, 04, 01, 01, 01, DateTimeKind.Local).ToDateOnly(),
            EnrollmentId = 4,
            EnrollmentName = "New vesting plan has Forfeiture records",
            HoursProfitYear = 1255.4m,
            WagesProfitYear = 12345.67m,
            Details = new List<RehireTransactionDetailResponse>
            {
                new RehireTransactionDetailResponse
                {
                    Forfeiture = (decimal)3254.14,
                    ProfitYear = (short)DateTime.Today.AddYears(-3).Year,
                    Remark = "Example remarks here",
                    HoursTransactionYear = 1255.4m,
                    WagesTransactionYear = 12345.67m,
                    SuggestedUnforfeiture = 77m
                }
            }
        };
    }


}
