using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class DistributionRunReportOnHoldEndpoint : ProfitSharingEndpoint<SortedPaginationRequestDto, Results<Ok<PaginatedResponseDto<DistributionsOnHoldResponse>>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;
    private readonly ILogger<DistributionRunReportOnHoldEndpoint> _logger;

    public DistributionRunReportOnHoldEndpoint(IDistributionService distributionService, ILogger<DistributionRunReportOnHoldEndpoint> logger) : base(Navigation.Constants.Distributions)
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


    protected override async Task<Results<Ok<PaginatedResponseDto<DistributionsOnHoldResponse>>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(
        SortedPaginationRequestDto req,
        CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn");

            var result = await _distributionService.GetDistributionsOnHoldAsync(req, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "distribution-report"),
                new("report_type", "on-hold"),
                new("endpoint", "DistributionRunReportOnHoldEndpoint"));

            if (result.IsSuccess)
            {
                var recordCount = result.Value?.Total ?? 0;
                EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
                    new("record_type", "on-hold-distributions"),
                    new("endpoint", "DistributionRunReportOnHoldEndpoint"));

                _logger.LogInformation("On-hold distributions report generated, returned {Count} results (correlation: {CorrelationId})",
                    recordCount, HttpContext.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning("On-hold distributions report failed - {Error} (correlation: {CorrelationId})",
                    result.Error, HttpContext.TraceIdentifier);
            }

            var httpResult = result.ToHttpResultWithValidation(Error.EntityNotFound("OnHoldDistributions"));
            this.RecordResponseMetrics(HttpContext, _logger, httpResult);

            return httpResult;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
