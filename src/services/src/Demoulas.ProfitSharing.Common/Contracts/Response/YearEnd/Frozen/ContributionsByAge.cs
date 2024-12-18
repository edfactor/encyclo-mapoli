using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public sealed record ContributionsByAge : ReportResponseBase<ContributionsByAgeDetail>
{
    public ContributionsByAge()
    {
        ReportName = "PROFIT SHARING CONTRIBUTIONS BY AGE";
        ReportDate = DateTimeOffset.Now;
    }

    public FrozenReportsByAgeRequest.Report ReportType { get; init; }

    public required short TotalEmployees { get; init; }
    public required decimal DistributionTotalAmount { get; init; }





    public static ContributionsByAge ResponseExample()
    {
        return new ContributionsByAge
        {
            ReportName = "PROFIT SHARING CONTRIBUTIONS BY AGE",
            ReportDate = DateTimeOffset.Now,
            DistributionTotalAmount = (decimal)1_855_156.09,
            TotalEmployees = 63,

            Response = new PaginatedResponseDto<ContributionsByAgeDetail>(new PaginationRequestDto())
            {
                Results = new List<ContributionsByAgeDetail> { ContributionsByAgeDetail.ResponseExample() }
            }
        };
    }
}
