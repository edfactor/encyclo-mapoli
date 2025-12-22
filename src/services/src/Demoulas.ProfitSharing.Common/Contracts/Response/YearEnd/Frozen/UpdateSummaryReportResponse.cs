using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public sealed record UpdateSummaryReportResponse : ReportResponseBase<UpdateSummaryReportDetail>
{
    [YearEndArchiveProperty]
    public int TotalNumberOfEmployees { get; set; }

    [YearEndArchiveProperty]
    public int TotalNumberOfBeneficiaries { get; set; }
    [YearEndArchiveProperty]
    public decimal TotalBeforeProfitSharingAmount { get; set; }
    [YearEndArchiveProperty]
    public decimal TotalBeforeVestedAmount { get; set; }
    [YearEndArchiveProperty]
    public decimal TotalAfterProfitSharingAmount { get; set; }
    [YearEndArchiveProperty]
    public decimal TotalAfterVestedAmount { get; set; }

    public static UpdateSummaryReportResponse ResponseExample()
    {
        return new UpdateSummaryReportResponse()
        {
            ReportName = "UPDATE SUMMARY FOR PROFIT SHARING",
            ReportDate = DateTimeOffset.Now,
            TotalNumberOfBeneficiaries = 2,
            TotalNumberOfEmployees = 3,
            TotalAfterProfitSharingAmount = 12500,
            TotalAfterVestedAmount = 7400,
            TotalBeforeProfitSharingAmount = 11500,
            TotalBeforeVestedAmount = 6800,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new Demoulas.Common.Contracts.Contracts.Response.PaginatedResponseDto<UpdateSummaryReportDetail>(new PaginationRequestDto())
            {
                Results = new List<UpdateSummaryReportDetail>() { UpdateSummaryReportDetail.ResponseExample() }
            }
        };
    }
}
