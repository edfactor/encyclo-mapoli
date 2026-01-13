using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
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

public sealed class DisbursementReportEndpoint : ProfitSharingEndpoint<ProfitYearRequest, Results<Ok<PaginatedResponseDto<DisbursementReportDetailResponse>>, NotFound, ProblemHttpResult>>
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

    public override Task<Results<Ok<PaginatedResponseDto<DisbursementReportDetailResponse>>, NotFound, ProblemHttpResult>> ExecuteAsync(ProfitYearRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _distributionService.GetDisbursementReport(req, ct).ConfigureAwait(false);

            // Record disbursement report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "disbursement-report"),
                new KeyValuePair<string, object?>("endpoint", "DisbursementReportEndpoint"));

            var recordCount = result.Value?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
                new KeyValuePair<string, object?>("record_type", "disbursement-details"),
                new KeyValuePair<string, object?>("endpoint", "DisbursementReportEndpoint"));

            _logger.LogInformation("Disbursement report retrieved for profit year {ProfitYear}, returned {Count} disbursement details (correlation: {CorrelationId})",
                req.ProfitYear, recordCount, HttpContext.TraceIdentifier);

            return result
                .ToHttpResult(Common.Contracts.Error.EntityNotFound("DisbursementReport"));
        }, "Ssn");
    }
}
