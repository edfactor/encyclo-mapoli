using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Api.Contracts;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
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

public sealed class DistributionRunReportEndpoint : ProfitSharingEndpoint<DistributionRunReportRequest, Results<Ok<PaginatedResponseDto<DistributionRunReportDetail>>, NotFound, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;
    private readonly ILogger<DistributionRunReportEndpoint> _logger;

    public DistributionRunReportEndpoint(IDistributionService distributionService, ILogger<DistributionRunReportEndpoint> logger) : base(Navigation.Constants.DistributionEditRunReport)
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

    public override async Task<Results<Ok<PaginatedResponseDto<DistributionRunReportDetail>>, NotFound, ProblemHttpResult>> ExecuteAsync(DistributionRunReportRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _distributionService.GetDistributionRunReport(req, ct).ConfigureAwait(false);

            // Record distribution run report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "distribution-run-report"),
                new KeyValuePair<string, object?>("endpoint", "DistributionRunReportEndpoint"));

            var recordCount = result.Value?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
                new KeyValuePair<string, object?>("record_type", "distribution-details"),
                new KeyValuePair<string, object?>("endpoint", "DistributionRunReportEndpoint"));

            _logger.LogInformation("Distribution run report retrieved, returned {Count} distribution details (correlation: {CorrelationId})",
                recordCount, HttpContext.TraceIdentifier);

            return result
                .ToHttpResult(Common.Contracts.Error.EntityNotFound("DistributionRunReport"));
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
