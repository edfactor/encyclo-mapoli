
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public record MilitaryAndRehireProfitSummaryResponse
{
    public required int BadgeNumber { get; set; }
    public required string? FullName { get; set; }
    public required string Ssn { get; set; }
    public required short StoreNumber { get; set; }
    public required DateOnly HireDate { get; set; }
    public required DateOnly? TerminationDate { get; set; }
    public required DateOnly ReHiredDate { get; set; }
    public required byte CompanyContributionYears { get; set; }
    public required decimal HoursCurrentYear { get; set; }
    public required short ProfitYear { get; set; }
    public required decimal Forfeiture { get; set; }
    public required string? Remark { get; set; }
    public required byte EnrollmentId { get; set; }
    public decimal NetBalanceLastYear { get; set; }
    public decimal VestedBalanceLastYear { get; set; }
    public char EmploymentStatusId { get; set; }
    public byte ProfitCodeId { get; set; }


    public static MilitaryAndRehireProfitSummaryResponse ResponseExample()
    {
        return new MilitaryAndRehireProfitSummaryResponse
        {
            BadgeNumber = 123,
            Ssn = "XXX-XX-1234",
            FullName = "Doe, John",
            StoreNumber = 22,
            HoursCurrentYear = (decimal)2345.6,
            CompanyContributionYears = 3,
            EnrollmentId = 4,
            EmploymentStatusId = 'a',
            NetBalanceLastYear = (decimal)1_234_567.89,
            VestedBalanceLastYear = (decimal)987_654.32,
            HireDate = DateTime.Today.AddYears(-5).ToDateOnly(),
            TerminationDate = DateTime.Today.AddYears(-3).ToDateOnly(),
            ReHiredDate = DateTime.Today.AddYears(-2).ToDateOnly(),
            Forfeiture = (decimal)3254.14,
            ProfitYear = (short)DateTime.Today.AddYears(-3).Year,
            Remark = "Example remarks here"
        };
    }
}
