using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

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

    public GetRmdsFactorByAgeEndpoint(IRmdsFactorService rmdsService)
        : base(Navigation.Constants.ManageRmdFactors)
    {
        _rmdsService = rmdsService;
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

    protected override async Task<Results<Ok<RmdsFactorDto>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        GetRmdsFactorByAgeRequest req,
        CancellationToken ct)
    {
        var result = await _rmdsService.GetByAgeAsync(req.Age, ct);
        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result);
    }
}
