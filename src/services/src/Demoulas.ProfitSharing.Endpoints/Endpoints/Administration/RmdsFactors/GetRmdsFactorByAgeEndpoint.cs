using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.RmdsFactors;

/// <summary>
/// GET endpoint to retrieve a single RMD factor by age.
/// </summary>
public sealed class GetRmdsFactorByAgeEndpoint
    : ProfitSharingEndpoint<GetRmdsFactorByAgeRequest, Results<Ok<RmdsFactorDto>, NotFound, ProblemHttpResult>>
{
    private readonly IRmdsFactorService _rmdsService;
    private readonly ILogger<GetRmdsFactorByAgeEndpoint> _logger;

    public GetRmdsFactorByAgeEndpoint(IRmdsFactorService rmdsService, ILogger<GetRmdsFactorByAgeEndpoint> logger)
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
            s.ExampleRequest = GetRmdsFactorByAgeRequest.RequestExample();
            s.Responses[200] = "Success - Returns RMD factor for the specified age";
            s.Responses[404] = "Not Found - No RMD factor exists for the specified age";
            s.Responses[403] = "Forbidden. Requires administrator access.";
        });

        Group<AdministrationGroup>();
    }

    protected override Task<Results<Ok<RmdsFactorDto>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        GetRmdsFactorByAgeRequest req,
        CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _rmdsService.GetByAgeAsync(req.Age, ct);

            if (result is null)
            {
                return TypedResults.NotFound();
            }

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "rmds-factor-by-age"),
                new("endpoint", nameof(GetRmdsFactorByAgeEndpoint)),
                new("age", req.Age.ToString()));

            return TypedResults.Ok(result);
        });
    }
}
