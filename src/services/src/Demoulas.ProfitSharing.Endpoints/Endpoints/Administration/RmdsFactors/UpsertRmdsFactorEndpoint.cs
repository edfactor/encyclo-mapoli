using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.RmdsFactors;

/// <summary>
/// POST/PUT endpoint to add or update an RMD factor.
/// </summary>
public sealed class UpsertRmdsFactorEndpoint
    : ProfitSharingEndpoint<RmdsFactorRequest, Results<Ok<RmdsFactorDto>, ProblemHttpResult>>
{
    private readonly IRmdsFactorService _rmdsService;

    public UpsertRmdsFactorEndpoint(IRmdsFactorService rmdsService)
        : base(Navigation.Constants.ManageRmdFactors)
    {
        _rmdsService = rmdsService;
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

    protected override async Task<Results<Ok<RmdsFactorDto>, ProblemHttpResult>> HandleRequestAsync(
        RmdsFactorRequest req,
        CancellationToken ct)
    {
        var result = await _rmdsService.UpsertAsync(req, ct);
        return TypedResults.Ok(result);
    }
}
