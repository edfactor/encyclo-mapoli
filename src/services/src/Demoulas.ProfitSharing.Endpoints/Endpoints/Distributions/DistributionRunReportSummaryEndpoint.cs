using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class DistributionRunReportSummaryEndpoint : ProfitSharingResponseEndpoint<Results<Ok<DistributionRunReportSummaryResponse[]>, NotFound, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;
    private readonly ILogger<DistributionRunReportSummaryEndpoint> _logger;

    public DistributionRunReportSummaryEndpoint(IDistributionService distributionService, ILogger<DistributionRunReportSummaryEndpoint> logger) : base(Navigation.Constants.DistributionEditRunReport)
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

    public override async Task<Results<Ok<DistributionRunReportSummaryResponse[]>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, new { });

            var result = await _distributionService.GetDistributionRunReportSummary(ct).ConfigureAwait(false);

            // Record distribution run report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "distribution-run-report-summary"),
                new KeyValuePair<string, object?>("endpoint", "DistributionRunReportSummaryEndpoint"));

            var recordCount = result.Value?.Length ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
                new KeyValuePair<string, object?>("record_type", "distribution-runs"),
                new KeyValuePair<string, object?>("endpoint", "DistributionRunReportSummaryEndpoint"));

            _logger.LogInformation("Distribution run report summary retrieved, returned {Count} distribution runs (correlation: {CorrelationId})",
                recordCount, HttpContext.TraceIdentifier);

            return result
                .ToHttpResult(Error.EntityNotFound("DistributionRun"));
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
