using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.Validation;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.CrossReference;

/// <summary>
/// Endpoint for retrieving comprehensive cross-reference validation data for the Master Update page.
/// Returns validation results for all checksum groups: Contributions, Earnings, Forfeitures, Distributions, and ALLOC transfers.
/// </summary>
public sealed class GetMasterUpdateValidationEndpoint
    : ProfitSharingEndpoint<YearRequest, Results<Ok<MasterUpdateCrossReferenceValidationResponse>, NotFound, ProblemHttpResult>>
{
    private readonly ICrossReferenceValidationService _crossReferenceValidationService;

    public GetMasterUpdateValidationEndpoint(
        ICrossReferenceValidationService crossReferenceValidationService)
        : base(Navigation.Constants.Unknown)
    {
        _crossReferenceValidationService = crossReferenceValidationService ?? throw new ArgumentNullException(nameof(crossReferenceValidationService));
    }

    public override void Configure()
    {
        Get("checksum/master-update/{ProfitYear}");
        Summary(s =>
        {
            s.Summary = "Get Master Update cross-reference validation data for a specific profit year";
            s.Description = "Retrieves comprehensive validation data for the Master Update page, including validation " +
                            "results for Contributions (PAY443/PAY444), Earnings, Forfeitures, Distributions, and ALLOC transfers. " +
                            "Returns detailed per-field validation with current values, expected values, and variance information.";
            s.RequestParam(r => r.ProfitYear, $"The profit year to validate (must be between 2020 and {DateTime.UtcNow.Year + 1})");
            s.ExampleRequest = new YearRequest { ProfitYear = 2024 };
            s.Response<MasterUpdateCrossReferenceValidationResponse>(200, "Validation data retrieved successfully");
            s.Response(404, "No archived reports found for the specified year");
            s.Response(400, $"Invalid profit year (must be between 2020 and {DateTime.UtcNow.Year + 1})");
            s.Response(403, "Forbidden - Requires year-end report viewing permissions");
        });
        Group<ValidationGroup>();
        Description(x => x
            .Produces<MasterUpdateCrossReferenceValidationResponse>(200)
            .Produces(404)
            .Produces(400)
            .Produces(403)
            .WithTags("Validation"));
    }

    protected override async Task<Results<Ok<MasterUpdateCrossReferenceValidationResponse>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        YearRequest req,
        CancellationToken ct)
    {
        var result = await _crossReferenceValidationService.ValidateMasterUpdateCrossReferencesAsync(
            req.ProfitYear,
            new Dictionary<string, decimal>(),
            ct);

        return result.Match<Results<Ok<MasterUpdateCrossReferenceValidationResponse>, NotFound, ProblemHttpResult>>(
            success => TypedResults.Ok(success),
            _ => result.Error?.Code == 104
                ? TypedResults.NotFound()
                : TypedResults.Problem(
                    detail: result.Error?.Description ?? "Unknown validation error",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Validation Error"));
    }
}
