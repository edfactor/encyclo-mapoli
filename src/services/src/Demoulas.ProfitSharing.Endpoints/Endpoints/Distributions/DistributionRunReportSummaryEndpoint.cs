using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class DistributionRunReportSummaryEndpoint : ProfitSharingEndpoint<EmptyRequest, Results<Ok<DistributionRunReportSummaryResponse[]>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;
    private readonly ILogger<DistributionRunReportSummaryEndpoint> _logger;

    public DistributionRunReportSummaryEndpoint(IDistributionService distributionService, ILogger<DistributionRunReportSummaryEndpoint> logger) : base(Navigation.Constants.Distributions)
    {
        _distributionService = distributionService;
        _logger = logger;
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

    protected override async Task<Results<Ok<DistributionRunReportSummaryResponse[]>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(EmptyRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _distributionService.GetDistributionRunReportSummaryAsync(ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "distribution-report"),
                new("report_type", "distribution-run-summary"),
                new("endpoint", "DistributionRunReportSummaryEndpoint"));

            if (result.IsSuccess)
            {
                var recordCount = result.Value?.Length ?? 0;
                EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
                    new("record_type", "distribution-run-summary"),
                    new("endpoint", "DistributionRunReportSummaryEndpoint"));

                _logger.LogInformation("Distribution run summary report generated, returned {Count} results (correlation: {CorrelationId})",
                    recordCount, HttpContext.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning("Distribution run summary report failed - {Error} (correlation: {CorrelationId})",
                    result.Error, HttpContext.TraceIdentifier);
            }

            var httpResult = result.ToHttpResultWithValidation(Error.EntityNotFound("DistributionRun"));
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
