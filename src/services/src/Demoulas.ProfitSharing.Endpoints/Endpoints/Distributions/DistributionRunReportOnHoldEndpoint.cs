using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class DistributionRunReportOnHoldEndpoint : ProfitSharingEndpoint<SortedPaginationRequestDto, Results<Ok<PaginatedResponseDto<DistributionsOnHoldResponse>>, NotFound, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;

    public DistributionRunReportOnHoldEndpoint(IDistributionService distributionService) : base(Navigation.Constants.DistributionEditRunReport)
    {
        _distributionService = distributionService;
    }

    public override void Configure()
    {
        Get("distribution-run-report/on-hold");
        Summary(s =>
        {
            s.Description = "Gets the on-hold portion of a distribution run.";
            s.Summary = "On-hold distributions in distribution run report";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<DistributionsOnHoldResponse>()
                {
                    DistributionsOnHoldResponse.SampleResponse()
                }
            } };
        });
        Group<DistributionGroup>();
    }


    protected override async Task<Results<Ok<PaginatedResponseDto<DistributionsOnHoldResponse>>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        SortedPaginationRequestDto req,
        CancellationToken ct)
    {
        var result = await _distributionService.GetDistributionsOnHold(req, ct);
        return result.ToHttpResult(Error.EntityNotFound("OnHoldDistributions"));
    }
}
