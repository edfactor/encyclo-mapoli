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
/// GET endpoint to retrieve all RMD factors by age.
/// </summary>
public sealed class GetAllRmdsFactorsEndpoint
    : ProfitSharingResponseEndpoint<Results<Ok<List<RmdsFactorDto>>, ProblemHttpResult>>
{
    private readonly IRmdsFactorService _rmdsService;
    private readonly ILogger<GetAllRmdsFactorsEndpoint> _logger;

    public GetAllRmdsFactorsEndpoint(
        IRmdsFactorService rmdsService,
        ILogger<GetAllRmdsFactorsEndpoint> logger)
        : base(Navigation.Constants.ManageRmdFactors)
    {
        _rmdsService = rmdsService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("rmds-factors");

        Summary(s =>
        {
            s.Summary = "Get All RMD Factors";
            s.Description = "Retrieves all Required Minimum Distribution (RMD) life expectancy factors by age. " +
                           "These factors are used to calculate minimum distributions from profit sharing accounts per IRS Publication 590-B.";
            s.Responses[200] = "Success - Returns list of all RMD factors by age (ordered by age)";
            s.Responses[403] = "Forbidden. Requires administrator access.";
        });
        
        Group<AdministrationGroup>();
    }

    public override async Task<Results<Ok<List<RmdsFactorDto>>, ProblemHttpResult>> ExecuteAsync(
        CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            var result = await _rmdsService.GetAllAsync(ct);

            // Record business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "get-all-rmds-factors"),
                new("endpoint", nameof(GetAllRmdsFactorsEndpoint)));

            EndpointTelemetry.RecordCountsProcessed.Record(result.Count,
                new("record_type", "rmds-factors"),
                new("endpoint", nameof(GetAllRmdsFactorsEndpoint)));

            _logger.LogInformation(
                "Retrieved {Count} RMD factors (correlation: {CorrelationId})",
                result.Count, HttpContext.TraceIdentifier);

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
