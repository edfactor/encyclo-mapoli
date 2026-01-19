using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class DistributionRunReportEndpoint : ProfitSharingEndpoint<DistributionRunReportRequest, Results<Ok<PaginatedResponseDto<DistributionRunReportDetail>>, NotFound, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;

    public DistributionRunReportEndpoint(IDistributionService distributionService) : base(Navigation.Constants.DistributionEditRunReport)
    {
        _distributionService = distributionService;
    }

    public override void Configure()
    {
        Get("distribution-run-report");
        Summary(s =>
        {
            s.Description = "Gets the distribution run report.";
            s.Summary = "Distribution run report";
            s.ExampleRequest = DistributionRunReportRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<DistributionRunReportDetail>()
                {
                    DistributionRunReportDetail.SampleResponse()
                }
            } };
        });
        Group<DistributionGroup>();
    }

    protected override async Task<Results<Ok<PaginatedResponseDto<DistributionRunReportDetail>>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        DistributionRunReportRequest req,
        CancellationToken ct)
    {
        var result = await _distributionService.GetDistributionRunReport(req, ct);
        return result.ToHttpResult(Common.Contracts.Error.EntityNotFound("DistributionRunReport"));
    }
}
