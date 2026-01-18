using Demoulas.ProfitSharing.Common.Contracts.Request.Validation;
using Demoulas.ProfitSharing.Common.Contracts.Response.Validation;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.CrossReference;

/// <summary>
/// Endpoint for retrieving validation data for Profit Sharing Summary Reports.
/// Returns validation results for Members, Balance, and Wages checksum groups.
/// </summary>
public sealed class GetProfitSharingReportValidationEndpoint
    : ProfitSharingEndpoint<ProfitSharingReportValidationRequest, Results<Ok<ValidationResponse>, NotFound, ProblemHttpResult>>
{
    private readonly ICrossReferenceValidationService _crossReferenceValidationService;

    public GetProfitSharingReportValidationEndpoint(
        ICrossReferenceValidationService crossReferenceValidationService)
        : base(Navigation.Constants.Unknown)
    {
        _crossReferenceValidationService = crossReferenceValidationService ?? throw new ArgumentNullException(nameof(crossReferenceValidationService));
    }

    public override void Configure()
    {
        Get("checksum/profit-sharing-report/{ProfitYear}/{ReportSuffix}/{UseFrozenData}");
        Summary(s =>
        {
            s.Summary = "Get Profit Sharing Report validation data for a specific profit year and report suffix";
            s.Description = "Retrieves validation data for Profit Sharing Summary Reports, including validation " +
                            "results for Members, Balance, and Wages against archived checksums. " +
                            "Returns detailed validation status and variance information.";
            s.RequestParam(r => r.ProfitYear, $"The profit year to validate (must be between 2020 and {DateTime.UtcNow.Year + 1})");
            s.RequestParam(r => r.ReportSuffix, "The report suffix (1-8) identifying the specific Profit Sharing Report");
            s.ExampleRequest = new ProfitSharingReportValidationRequest { ProfitYear = 2024, ReportSuffix = "1" };
            s.Response<ValidationResponse>(200, "Validation data retrieved successfully");
            s.Response(404, "No archived checksums found for the specified year and report");
            s.Response(400, $"Invalid profit year (must be between 2020 and {DateTime.UtcNow.Year + 1}) or invalid report suffix");
            s.Response(403, "Forbidden - Requires year-end report viewing permissions");
        });
        Group<ValidationGroup>();
        Description(x => x
            .Produces<ValidationResponse>(200)
            .Produces(404)
            .Produces(400)
            .Produces(403)
            .WithTags("Validation"));
    }

    protected override async Task<Results<Ok<ValidationResponse>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        ProfitSharingReportValidationRequest req,
        CancellationToken ct)
    {
        var result = await _crossReferenceValidationService.ValidateProfitSharingReport(
            req.ProfitYear,
            req.ReportSuffix,
            req.UseFrozenData,
            ct);

        return result.Match<Results<Ok<ValidationResponse>, NotFound, ProblemHttpResult>>(
            success => TypedResults.Ok(success),
            _ => result.Error?.Code == 104
                ? TypedResults.NotFound()
                : TypedResults.Problem(
                    detail: result.Error?.Description ?? "Unknown validation error",
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Validation Error"));
    }
}
