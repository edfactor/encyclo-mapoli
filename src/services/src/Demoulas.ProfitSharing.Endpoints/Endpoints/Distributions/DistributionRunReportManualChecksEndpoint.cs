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

public sealed class DistributionRunReportManualChecksEndpoint : ProfitSharingEndpoint<SortedPaginationRequestDto, Results<Ok<PaginatedResponseDto<ManualChecksWrittenResponse>>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;
    private readonly ILogger<DistributionRunReportManualChecksEndpoint> _logger;

    public DistributionRunReportManualChecksEndpoint(IDistributionService distributionService, ILogger<DistributionRunReportManualChecksEndpoint> logger) : base(Navigation.Constants.Distributions)
    {
        _distributionService = distributionService;
        _logger = logger;
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

    protected override async Task<Results<Ok<PaginatedResponseDto<ManualChecksWrittenResponse>>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(
        SortedPaginationRequestDto req,
        CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn");

            var result = await _distributionService.GetManualCheckDistributionsAsync(req, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "distribution-report"),
                new("report_type", "manual-checks"),
                new("endpoint", "DistributionRunReportManualChecksEndpoint"));

            if (result.IsSuccess)
            {
                var recordCount = result.Value?.Total ?? 0;
                EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
                    new("record_type", "manual-check-distributions"),
                    new("endpoint", "DistributionRunReportManualChecksEndpoint"));

                _logger.LogInformation("Manual checks distributions report generated, returned {Count} results (correlation: {CorrelationId})",
                    recordCount, HttpContext.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning("Manual checks distributions report failed - {Error} (correlation: {CorrelationId})",
                    result.Error, HttpContext.TraceIdentifier);
            }

            var httpResult = result.ToHttpResultWithValidation(Error.EntityNotFound("ManualCheckDistributions"));
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
