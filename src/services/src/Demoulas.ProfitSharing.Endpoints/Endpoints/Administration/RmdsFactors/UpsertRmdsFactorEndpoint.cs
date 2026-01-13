using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.RmdsFactors;

/// <summary>
/// POST/PUT endpoint to add or update an RMD factor.
/// </summary>
public sealed class UpsertRmdsFactorEndpoint
    : ProfitSharingEndpoint<RmdsFactorRequest, Results<Ok<RmdsFactorDto>, ProblemHttpResult>>
{
    private readonly IRmdsFactorService _rmdsService;
    private readonly ILogger<UpsertRmdsFactorEndpoint> _logger;

    public UpsertRmdsFactorEndpoint(
        IRmdsFactorService rmdsService,
        ILogger<UpsertRmdsFactorEndpoint> logger)
        : base(Navigation.Constants.ManageRmdFactors)
    {
        _rmdsService = rmdsService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("rmds-factors");

        Summary(s =>
        {
            s.Summary = "Add or Update RMD Factor";
            s.Description = "Adds a new or updates an existing Required Minimum Distribution (RMD) life expectancy factor for a specific age. " +
                           "If the age already exists, the factor will be updated. Otherwise, a new entry will be created.";
            s.ExampleRequest = RmdsFactorRequest.RequestExample();
            s.Responses[200] = "Success - RMD factor added or updated";
            s.Responses[400] = "Bad Request - Invalid age or factor value";
            s.Responses[403] = "Forbidden. Requires administrator access.";
        });

        Group<AdministrationGroup>();
    }

    public override async Task<Results<Ok<RmdsFactorDto>, ProblemHttpResult>> ExecuteAsync(
        RmdsFactorRequest req,
        CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _rmdsService.UpsertAsync(req, ct);

            // Record business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "upsert-rmds-factor"),
                new("endpoint", nameof(UpsertRmdsFactorEndpoint)));

            _logger.LogInformation(
                "Upserted RMD factor for age {Age}: Factor={Factor} (correlation: {CorrelationId})",
                req.Age, req.Factor, HttpContext.TraceIdentifier);

            this.RecordResponseMetrics(HttpContext, _logger, result);
            return TypedResults.Ok(result);
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
