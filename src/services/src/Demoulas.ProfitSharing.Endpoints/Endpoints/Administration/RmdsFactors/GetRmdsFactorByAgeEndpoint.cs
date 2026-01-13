using Demoulas.ProfitSharing.Common.Contracts.Response;
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

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.RmdsFactors;

/// <summary>
/// Request for getting RMD factor by age.
/// </summary>
public sealed record GetRmdsFactorByAgeRequest
{
    /// <summary>
    /// Age to retrieve RMD factor for.
    /// </summary>
    public required byte Age { get; init; }
}

/// <summary>
/// GET endpoint to retrieve a single RMD factor by age.
/// </summary>
public sealed class GetRmdsFactorByAgeEndpoint
    : ProfitSharingEndpoint<GetRmdsFactorByAgeRequest, Results<Ok<RmdsFactorDto>, NotFound, ProblemHttpResult>>
{
    private readonly IRmdsFactorService _rmdsService;
    private readonly ILogger<GetRmdsFactorByAgeEndpoint> _logger;

    public GetRmdsFactorByAgeEndpoint(
        IRmdsFactorService rmdsService,
        ILogger<GetRmdsFactorByAgeEndpoint> logger)
        : base(Navigation.Constants.ManageRmdFactors)
    {
        _rmdsService = rmdsService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("rmds-factors/{Age}");

        Summary(s =>
        {
            s.Summary = "Get RMD Factor by Age";
            s.Description = "Retrieves the Required Minimum Distribution (RMD) life expectancy factor for a specific age. " +
                           "Formula: RMD Amount = Account Balance ÷ Factor";
            s.ExampleRequest = new GetRmdsFactorByAgeRequest { Age = 73 };
            s.Responses[200] = "Success - Returns RMD factor for the specified age";
            s.Responses[404] = "Not Found - No RMD factor exists for the specified age";
            s.Responses[403] = "Forbidden. Requires administrator access.";
        });

        Group<AdministrationGroup>();
    }

    public override async Task<Results<Ok<RmdsFactorDto>, NotFound, ProblemHttpResult>> ExecuteAsync(
        GetRmdsFactorByAgeRequest req,
        CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _rmdsService.GetByAgeAsync(req.Age, ct);

            if (result is null)
            {
                _logger.LogWarning(
                    "RMD factor not found for age {Age} (correlation: {CorrelationId})",
                    req.Age, HttpContext.TraceIdentifier);

                return TypedResults.NotFound();
            }

            // Record business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "get-rmds-factor-by-age"),
                new("endpoint", nameof(GetRmdsFactorByAgeEndpoint)));

            _logger.LogInformation(
                "Retrieved RMD factor for age {Age}: Factor={Factor} (correlation: {CorrelationId})",
                req.Age, result.Factor, HttpContext.TraceIdentifier);

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
