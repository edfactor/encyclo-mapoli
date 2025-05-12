using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;
internal sealed record MilitaryAndRehireProfitSummaryQueryResponse
{
    internal required int BadgeNumber { get; set; }
    internal required string? FullName { get; set; }
    internal required int Ssn { get; set; }
    internal required short StoreNumber { get; set; }
    internal required DateOnly HireDate { get; set; }
    internal required DateOnly? TerminationDate { get; set; }
    internal required DateOnly ReHiredDate { get; set; }
    internal required byte CompanyContributionYears { get; set; }
    internal required decimal HoursCurrentYear { get; set; }
    internal required short ProfitYear { get; set; }
    internal required decimal Forfeiture { get; set; }
    internal required string? Remark { get; set; }
    internal required byte EnrollmentId { get; set; }
    internal required string EnrollmentName { get; set; }
    internal decimal? NetBalanceLastYear { get; set; }
    internal decimal? VestedBalanceLastYear { get; set; }
    internal char EmploymentStatusId { get; set; }
    internal byte ProfitCodeId { get; set; }
    internal string? EmploymentStatus { get; set; }


    internal static MilitaryAndRehireProfitSummaryResponse ResponseExample()
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
