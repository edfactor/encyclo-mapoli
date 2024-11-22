using System.Collections.Frozen;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record ContributionsByAge : ReportResponseBase<ContributionsByAgeDetail>
{
    public ContributionsByAge()
    {
        ReportName = "PROFIT SHARING CONTRIBUTIONS BY AGE";
        ReportDate = DateTimeOffset.Now;
    }

    public FrozenReportsByAgeRequest.Report ReportType { get; set; }

    public decimal RegularTotalAmount { get; set; }

    public decimal DistributionTotalAmount { get; set; }





    public static ContributionsByAge ResponseExample()
    {
        return new ContributionsByAge
        {
            ReportName = "PROFIT SHARING CONTRIBUTIONS BY AGE",
            ReportDate = DateTimeOffset.Now,
            RegularTotalAmount = (decimal)1_855_156.09,
            
            Response = new PaginatedResponseDto<ContributionsByAgeDetail>(new PaginationRequestDto())
            {
                Results = new List<ContributionsByAgeDetail> { ContributionsByAgeDetail.ResponseExample() }
            }
        };
    }
}
