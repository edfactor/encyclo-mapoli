using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class DistributionRunReportSummaryEndpoint : ProfitSharingResponseEndpoint<Results<Ok<DistributionRunReportSummaryResponse[]>, NotFound, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;

    public DistributionRunReportSummaryEndpoint(IDistributionService distributionService) : base(Navigation.Constants.DistributionEditRunReport)
    {
        _distributionService = distributionService;
    }

    public override void Configure()
    {
        Get("distribution-run-report/summary");
        Summary(s =>
        {
            s.Description = "Gets the summary portion of a distribution run.";
            s.Summary = "Summary of distribution run report";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<DistributionRunReportSummaryResponse>()
                {
                    DistributionRunReportSummaryResponse.SampleResponse()
                }
            } };
        });
        Group<DistributionGroup>();
    }

    protected override async Task<Results<Ok<DistributionRunReportSummaryResponse[]>, NotFound, ProblemHttpResult>> HandleRequestAsync(CancellationToken ct)
    {
        var result = await _distributionService.GetDistributionRunReportSummary(ct);
        return result.ToHttpResult(Error.EntityNotFound("DistributionRun"));
    }
}
