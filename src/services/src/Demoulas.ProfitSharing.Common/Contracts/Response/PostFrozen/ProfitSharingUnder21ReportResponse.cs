namespace Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;

public sealed record ProfitSharingUnder21ReportResponse:ReportResponseBase<ProfitSharingUnder21ReportDetail>
{
    public static readonly string REPORT_NAME = "PROFIT SHARING UNDER AGE 21 REPORT";
    public ProfitSharingUnder21ReportResponse()
    {
        this.ReportName = REPORT_NAME;
    }

    public required ProfitSharingUnder21TotalForStatus ActiveTotals { get; set; }
    public required ProfitSharingUnder21TotalForStatus InactiveTotals { get; set; }
    public required ProfitSharingUnder21TotalForStatus TerminatedTotals { get; set; }
    public int TotalUnder21 { get; set; }

    public static ProfitSharingUnder21ReportResponse ResponseExample()
    {
        return new ProfitSharingUnder21ReportResponse()
        {
            ReportName = REPORT_NAME,
            ReportDate = DateTime.UtcNow,
            ActiveTotals = new ProfitSharingUnder21TotalForStatus(4, 1, 5),
            InactiveTotals = new ProfitSharingUnder21TotalForStatus(3, 9, 2),
            TerminatedTotals = new ProfitSharingUnder21TotalForStatus(6, 8, 7),
            TotalUnder21 = 45,
            Response = new Demoulas.Common.Contracts.Contracts.Response.PaginatedResponseDto<ProfitSharingUnder21ReportDetail>()
            {
                Total = 1,
                Results = new List<ProfitSharingUnder21ReportDetail>() { ProfitSharingUnder21ReportDetail.ResponseExample() }
            }
        };
    }

}
public record ProfitSharingUnder21ReportDetail(short StoreNumber, int BadgeNumber, string FirstName, string LastName, string Ssn, byte ProfitSharingYears, bool IsNew, decimal ThisYearHours, decimal LastYearHours, DateOnly HireDate, DateOnly? FullTimeDate, DateOnly? TerminationDate, DateOnly DateOfBirth, short Age, char EmploymentStatusId, decimal CurrentBalance, byte EnrollmentId) 
{ 
    public static ProfitSharingUnder21ReportDetail ResponseExample()
    {
        return new ProfitSharingUnder21ReportDetail(44, 700312, "Wendy", "Johnson", "xxx-xx-4002", 2, false, 1500, 1402, new DateOnly(2023, 4, 1), null, null, new DateOnly(1991,9,24), 20, 'a', 14002, 2);
    }
};
public record ProfitSharingUnder21TotalForStatus(int TotalVested, int PartiallyVested, int PartiallyVestedButLessThanThreeYears);