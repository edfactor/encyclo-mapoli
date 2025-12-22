using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

[NoMemberDataExposed]
public sealed record ContributionsByAge : ReportResponseBase<ContributionsByAgeDetail>
{
    public ContributionsByAge()
    {
        ReportName = "PROFIT SHARING CONTRIBUTIONS BY AGE";
        ReportDate = DateTimeOffset.Now;
    }

    public FrozenReportsByAgeRequest.Report ReportType { get; init; }

    public required ushort TotalEmployees { get; init; }
    public required decimal TotalAmount { get; init; }





    public static ContributionsByAge ResponseExample()
    {
        return new ContributionsByAge
        {
            ReportName = "PROFIT SHARING CONTRIBUTIONS BY AGE",
            ReportDate = DateTimeOffset.Now,
            TotalAmount = (decimal)1_855_156.09,
            TotalEmployees = 63,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new PaginatedResponseDto<ContributionsByAgeDetail>(new PaginationRequestDto())
            {
                Results = new List<ContributionsByAgeDetail> { ContributionsByAgeDetail.ResponseExample() }
            }
        };
    }
}
