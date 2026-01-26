using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class DisbursementReportEndpoint : ProfitSharingEndpoint<ProfitYearRequest, Results<Ok<PaginatedResponseDto<DisbursementReportDetailResponse>>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;
    private readonly ILogger<DisbursementReportEndpoint> _logger;

    public DisbursementReportEndpoint(IDistributionService distributionService, ILogger<DisbursementReportEndpoint> logger) : base(Navigation.Constants.Distributions)
    {
        _distributionService = distributionService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("disbursement-report");
        Summary(s =>
        {
            s.Description = "Gets the disbursement report with distribution details by profit year.";
            s.Summary = "Disbursement report - QPAY078";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new List<DisbursementReportDetailResponse>()
                {
                    DisbursementReportDetailResponse.SampleResponse()
                }
            } };
        });
        Group<DistributionGroup>();
    }

    protected override async Task<Results<Ok<PaginatedResponseDto<DisbursementReportDetailResponse>>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(
        ProfitYearRequest req,
        CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _distributionService.GetDisbursementReportAsync(req, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "distribution-report"),
                new("report_type", "disbursement"),
                new("endpoint", "DisbursementReportEndpoint"));

            if (result.IsSuccess)
            {
                var recordCount = result.Value?.Total ?? 0;
                EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
                    new("record_type", "disbursement-report"),
                    new("endpoint", "DisbursementReportEndpoint"));

                _logger.LogInformation("Disbursement report generated for year {ProfitYear}, returned {Count} results (correlation: {CorrelationId})",
                    req.ProfitYear, recordCount, HttpContext.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning("Disbursement report failed for year {ProfitYear} - {Error} (correlation: {CorrelationId})",
                    req.ProfitYear, result.Error, HttpContext.TraceIdentifier);
            }

            var httpResult = result.ToHttpResultWithValidation(Error.EntityNotFound("DisbursementReport"));
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
