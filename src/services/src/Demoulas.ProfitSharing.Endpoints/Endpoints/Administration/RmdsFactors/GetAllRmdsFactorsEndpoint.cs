using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.RmdsFactors;

/// <summary>
/// GET endpoint to retrieve all RMD factors by age.
/// </summary>
public sealed class GetAllRmdsFactorsEndpoint
    : ProfitSharingResponseEndpoint<Results<Ok<List<RmdsFactorDto>>, ProblemHttpResult>>
{
    private readonly IRmdsFactorService _rmdsService;

    public GetAllRmdsFactorsEndpoint(IRmdsFactorService rmdsService)
        : base(Navigation.Constants.ManageRmdFactors)
    {
        _rmdsService = rmdsService;
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

    protected override async Task<Results<Ok<List<RmdsFactorDto>>, ProblemHttpResult>> HandleRequestAsync(CancellationToken ct)
    {
        var result = await _rmdsService.GetAllAsync(ct);
        return TypedResults.Ok(result);
    }
}
