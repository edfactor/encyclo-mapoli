using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;

public class DistributionsAndForfeitureEndpoint : ProfitSharingEndpoint<DistributionsAndForfeituresRequest, Results<Ok<DistributionsAndForfeitureTotalsResponse>, NotFound, ProblemHttpResult>>
{
    private readonly ICleanupReportService _cleanupReportService;
    private readonly ILogger<DistributionsAndForfeitureEndpoint> _logger;

    public DistributionsAndForfeitureEndpoint(ICleanupReportService cleanupReportService, ILogger<DistributionsAndForfeitureEndpoint> logger)
        : base(Navigation.Constants.DistributionsAndForfeitures)
    {
        _cleanupReportService = cleanupReportService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("distributions-and-forfeitures");
        Summary(s =>
        {
            s.Summary = "Lists distributions and forfeitures for a date range";
            s.ExampleRequest = new DistributionsAndForfeituresRequest() { Skip = 0, Take = 100 };
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, DistributionsAndForfeitureTotalsResponse.ResponseExample() }
            };
        });
        Group<YearEndGroup>();
    }

    public override Task<Results<Ok<DistributionsAndForfeitureTotalsResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(DistributionsAndForfeituresRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var serviceResult = await _cleanupReportService.GetDistributionsAndForfeitureAsync(req, ct);

            // Record year-end cleanup report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-cleanup-distributions-forfeitures"),
                new("endpoint", nameof(DistributionsAndForfeitureEndpoint)));

            if (serviceResult is { IsSuccess: true, Value.Response.Results: not null })
            {
                var resultCount = serviceResult.Value.Response.Results.Count();
                EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                    new("record_type", "distributions-forfeitures-cleanup"),
                    new("endpoint", nameof(DistributionsAndForfeitureEndpoint)));

                _logger.LogInformation("Year-end cleanup report for distributions and forfeitures generated, returned {Count} records (correlation: {CorrelationId})",
                    resultCount, HttpContext.TraceIdentifier);

                return serviceResult.ToHttpResult();
            }

            return serviceResult.ToHttpResult(Error.NoPayProfitsDataAvailable);
        });
    }
}
