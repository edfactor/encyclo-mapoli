using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.Validation;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Validation;

/// <summary>
/// Endpoint for retrieving comprehensive cross-reference validation data for the Master Update page.
/// Returns validation results for all checksum groups: Contributions, Earnings, Forfeitures, Distributions, and ALLOC transfers.
/// </summary>
public sealed class GetMasterUpdateValidationEndpoint
    : ProfitSharingEndpoint<YearRequest, Results<Ok<MasterUpdateCrossReferenceValidationResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IChecksumValidationService _validationService;
    private readonly ILogger<GetMasterUpdateValidationEndpoint> _logger;

    public GetMasterUpdateValidationEndpoint(
        IChecksumValidationService validationService,
        ILogger<GetMasterUpdateValidationEndpoint> logger)
        : base(Navigation.Constants.Unknown)
    {
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Get("validation/checksum/master-update/{profitYear}");
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
        });
        // Group assignment removed - validation endpoints don't need group
        Description(d =>
        {
            d.WithTags("Validation");
        });
        AllowAnonymous();
    }

    public override Task<Results<Ok<MasterUpdateCrossReferenceValidationResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(
        YearRequest req,
        CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var profitYear = req.ProfitYear;

            // Note: Year validation is handled by ProfitYearRequestValidator (2020 to current year + 1)
            // This ensures requests are validated before reaching this point

            // For standalone validation endpoint, pass empty dictionary
            // This will cause validation service to compare only archived report checksums
            var emptyCurrentValues = new Dictionary<string, decimal>();

            var result = await _validationService.ValidateMasterUpdateCrossReferencesAsync(
                profitYear,
                emptyCurrentValues,
                ct);

            return result.Match<Results<Ok<MasterUpdateCrossReferenceValidationResponse>, NotFound, ProblemHttpResult>>(
                success => TypedResults.Ok(success),
                problemDetails =>
                {
                    // Check if this is a "not found" error (code 104)
                    if (result.Error?.Code == 104)
                    {
                        return TypedResults.NotFound();
                    }

                    return TypedResults.Problem(
                        detail: result.Error?.Description ?? "Unknown validation error",
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "Validation Error"
                    );
                }
            );
        });
    }
}
