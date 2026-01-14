using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class DistributionRunReportOnHoldEndpoint : ProfitSharingEndpoint<SortedPaginationRequestDto, Results<Ok<PaginatedResponseDto<DistributionsOnHoldResponse>>, NotFound, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;
    private readonly ILogger<DistributionRunReportOnHoldEndpoint> _logger;

    public DistributionRunReportOnHoldEndpoint(IDistributionService distributionService, ILogger<DistributionRunReportOnHoldEndpoint> logger) : base(Navigation.Constants.DistributionEditRunReport)
    {
        _distributionService = distributionService;
        _logger = logger;
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


    public override Task<Results<Ok<PaginatedResponseDto<DistributionsOnHoldResponse>>, NotFound, ProblemHttpResult>> ExecuteAsync(SortedPaginationRequestDto req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _distributionService.GetDistributionsOnHold(req, ct);

            // Record distribution on-hold report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "distribution-run-report-on-hold"),
                new KeyValuePair<string, object?>("endpoint", "DistributionRunReportOnHoldEndpoint"));

            _logger.LogInformation("Distribution on-hold report retrieved(correlation: {CorrelationId})",
                 HttpContext.TraceIdentifier);

            return result
                .ToHttpResult(Error.EntityNotFound("OnHoldDistributions"));
        }); // No sensitive fields accessed
    }
}
