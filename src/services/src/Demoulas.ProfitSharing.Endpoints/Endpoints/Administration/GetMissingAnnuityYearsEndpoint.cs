using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

/// <summary>
/// Endpoint to check which years have complete annuity rate data.
/// Returns completeness status for a range of years (default: current year and previous 5 years).
/// </summary>
public sealed class GetMissingAnnuityYearsEndpoint : ProfitSharingEndpoint<GetMissingAnnuityYearsRequest, Results<Ok<MissingAnnuityYearsResponse>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IAnnuityRatesService _annuityRatesService;

    public GetMissingAnnuityYearsEndpoint(IAnnuityRatesService annuityRatesService)
        : base(Navigation.Constants.ManageAnnuityRates)
    {
        _annuityRatesService = annuityRatesService;
    }

    public override void Configure()
    {
        Get("annuity-rates/missing-years");
        Summary(s =>
        {
            s.Summary = "Checks which years have complete annuity rate data (all required ages defined).";
            s.Description = "Returns completeness status for a range of years. Defaults to current year and previous 5 years if no range specified.";
            s.ExampleRequest = GetMissingAnnuityYearsRequest.RequestExample();
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<MissingAnnuityYearsResponse>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(
        GetMissingAnnuityYearsRequest req,
        CancellationToken ct)
    {
        var result = await _annuityRatesService.GetMissingAnnuityYearsAsync(req, ct);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value!)
            : result.ToHttpResultWithValidation();
    }
}
