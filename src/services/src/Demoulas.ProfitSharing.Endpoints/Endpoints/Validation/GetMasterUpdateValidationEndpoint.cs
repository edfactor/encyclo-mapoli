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
    private readonly ICrossReferenceValidationService _crossReferenceValidationService;
    private readonly ILogger<GetMasterUpdateValidationEndpoint> _logger;

    public GetMasterUpdateValidationEndpoint(
        ICrossReferenceValidationService crossReferenceValidationService,
        ILogger<GetMasterUpdateValidationEndpoint> logger)
        : base(Navigation.Constants.Unknown)
    {
        _crossReferenceValidationService = crossReferenceValidationService ?? throw new ArgumentNullException(nameof(crossReferenceValidationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Get("checksum/master-update/{profitYear}");
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

    public override Task<Results<Ok<MasterUpdateCrossReferenceValidationResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(
        YearRequest req,
        CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var profitYear = req.ProfitYear;

            // Note: Year validation is handled by ProfitYearRequestValidator (2020 to current year + 1)
            // This ensures requests are validated before reaching this point

            // Get archived values without comparison - UI will do the comparison
            // PS-1721: Now using ICrossReferenceValidationService with empty dictionary to get ExpectedValues
            var result = await _crossReferenceValidationService.ValidateMasterUpdateCrossReferencesAsync(
                profitYear,
                new Dictionary<string, decimal>(), // Empty dictionary - we only want archived ExpectedValues
                ct);

            if (result.IsSuccess && result.Value != null)
            {
                // Record business metrics for successful validation retrieval
                Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                    new KeyValuePair<string, object?>("operation", "master-update-validation-retrieval"),
                    new KeyValuePair<string, object?>("endpoint", nameof(GetMasterUpdateValidationEndpoint)),
                    new KeyValuePair<string, object?>("profit_year", profitYear));

                // Record validation metrics
                var validationGroupCount = result.Value.ValidationGroups.Count;
                var validGroupCount = result.Value.ValidationGroups.Count(g => g.IsValid);

                Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.RecordCountsProcessed.Record(
                    validationGroupCount,
                    new KeyValuePair<string, object?>("record_type", "validation-groups"),
                    new KeyValuePair<string, object?>("endpoint", nameof(GetMasterUpdateValidationEndpoint)));
            }

            return result.Match<Results<Ok<MasterUpdateCrossReferenceValidationResponse>, NotFound, ProblemHttpResult>>(
                success => TypedResults.Ok(success),
                problemDetails =>
                {
                    // Check if this is a "not found" error (code 104)
                    if (result.Error?.Code == 104)
                    {
                        _logger.LogWarning(
                            "No archived reports found for profit year {ProfitYear}",
                            profitYear);
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
