using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class DistributionRunReportManualChecksEndpoint : ProfitSharingEndpoint<SortedPaginationRequestDto, Results<Ok<PaginatedResponseDto<ManualChecksWrittenResponse>>, NotFound, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;

    public DistributionRunReportManualChecksEndpoint(IDistributionService distributionService) : base(Navigation.Constants.DistributionEditRunReport)
    {
        _distributionService = distributionService;
    }

    public override void Configure()
    {
        Get("distribution-run-report/manual-checks");
        Summary(s =>
        {
            s.Description = "Gets the manual check portion of a distribution run.";
            s.Summary = "Manual check distributions in distribution run report";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<ManualChecksWrittenResponse>()
                {
                    ManualChecksWrittenResponse.ResponseExample()
                }
            } };
        });
        Group<DistributionGroup>();
    }

    protected override async Task<Results<Ok<PaginatedResponseDto<ManualChecksWrittenResponse>>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        SortedPaginationRequestDto req,
        CancellationToken ct)
    {
        var result = await _distributionService.GetManualCheckDistributions(req, ct);
        return result.ToHttpResult(Error.EntityNotFound("ManualCheckDistributions"));
    }
}
