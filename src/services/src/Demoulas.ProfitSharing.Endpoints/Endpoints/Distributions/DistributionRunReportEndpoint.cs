using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class DistributionRunReportEndpoint : ProfitSharingEndpoint<DistributionRunReportRequest, Results<Ok<PaginatedResponseDto<DistributionRunReportDetail>>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;
    private readonly ILogger<DistributionRunReportEndpoint> _logger;

    public DistributionRunReportEndpoint(IDistributionService distributionService, ILogger<DistributionRunReportEndpoint> logger) : base(Navigation.Constants.Distributions)
    {
        _distributionService = distributionService;
        _logger = logger;
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

    protected override async Task<Results<Ok<PaginatedResponseDto<DistributionRunReportDetail>>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(
        DistributionRunReportRequest req,
        CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _distributionService.GetDistributionRunReportAsync(req, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "distribution-report"),
                new("report_type", "distribution-run"),
                new("endpoint", "DistributionRunReportEndpoint"));

            if (result.IsSuccess)
            {
                var recordCount = result.Value?.Total ?? 0;
                EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
                    new("record_type", "distribution-run-report"),
                    new("endpoint", "DistributionRunReportEndpoint"));

                _logger.LogInformation("Distribution run report generated, returned {Count} results (correlation: {CorrelationId})",
                    recordCount, HttpContext.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning("Distribution run report failed - {Error} (correlation: {CorrelationId})",
                    result.Error, HttpContext.TraceIdentifier);
            }

            var httpResult = result.ToHttpResultWithValidation(Error.EntityNotFound("DistributionRunReport"));
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
