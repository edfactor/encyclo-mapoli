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
/// Endpoint for validating ALLOC/PAID ALLOC transfer balance per Balance Matrix Rule 2.
/// Validates that incoming QDRO beneficiary allocations (ALLOC) and outgoing XFER beneficiary 
/// allocations (PAID ALLOC) sum to zero for data integrity.
/// </summary>
public sealed class ValidateAllocTransfersEndpoint
    : ProfitSharingEndpoint<YearRequest, Results<Ok<CrossReferenceValidationGroup>, NotFound, ProblemHttpResult>>
{
    private readonly IBalanceValidationService _balanceValidationService;
    private readonly ILogger<ValidateAllocTransfersEndpoint> _logger;

    public ValidateAllocTransfersEndpoint(
        IBalanceValidationService balanceValidationService,
        ILogger<ValidateAllocTransfersEndpoint> logger)
        : base(Navigation.Constants.Unknown)
    {
        _balanceValidationService = balanceValidationService ?? throw new ArgumentNullException(nameof(balanceValidationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Get("balance-validation/alloc-transfers/{profitYear}");
        Summary(s =>
        {
            s.Summary = "Validate ALLOC/PAID ALLOC transfer balance";
            s.Description = "Validates that ALLOC (Incoming QDRO Beneficiary, code 6) and PAID ALLOC " +
                            "(Outgoing XFER Beneficiary, code 5) transactions sum to zero for a given profit year. " +
                            "This is Balance Matrix Rule 2 - a critical data integrity check. " +
                            "Returns detailed breakdown of incoming allocations, outgoing allocations, and net transfer amount.";
            s.RequestParam(r => r.ProfitYear, $"The profit year to validate (must be between 2020 and {DateTime.UtcNow.Year + 1})");
            s.ExampleRequest = new YearRequest { ProfitYear = 2024 };
            s.Response<CrossReferenceValidationGroup>(200, "Validation completed. Check IsValid to determine if transfers balance.");
            s.Response(404, "No profit data found for the specified year");
            s.Response(400, $"Invalid profit year (must be between 2020 and {DateTime.UtcNow.Year + 1})");
            s.Response(403, "Forbidden - Requires appropriate role permissions");
        });
        Group<ValidationGroup>();
        Description(x => x
            .Produces<CrossReferenceValidationGroup>(200)
            .Produces(404)
            .Produces(400)
            .Produces(403));
    }

    public override async Task<Results<Ok<CrossReferenceValidationGroup>, NotFound, ProblemHttpResult>> ExecuteAsync(
        YearRequest req,
        CancellationToken ct)
    {
        _logger.LogInformation(
            "Validating ALLOC/PAID ALLOC transfer balance for year {ProfitYear}",
            req.ProfitYear);

        // Note: Year validation is handled by ProfitYearRequestValidator (2020 to current year + 1)
        // This ensures requests are validated before reaching this point

        var result = await _balanceValidationService.ValidateAllocTransfersAsync(req.ProfitYear, ct);

        if (!result.IsSuccess)
        {
            _logger.LogError(
                "ALLOC transfer validation failed for year {ProfitYear}: {Error}",
                req.ProfitYear,
                result.Error?.Description);

            return TypedResults.Problem(
                title: "Validation Error",
                detail: result.Error?.Description ?? "Unknown error during validation",
                statusCode: 500);
        }

        if (result.Value == null)
        {
            _logger.LogWarning("No validation result returned for year {ProfitYear}", req.ProfitYear);
            return TypedResults.NotFound();
        }

        // Record business metrics
        Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new KeyValuePair<string, object?>("operation", "alloc-transfer-validation-endpoint"),
            new KeyValuePair<string, object?>("endpoint.category", "balance-validation"),
            new KeyValuePair<string, object?>("profit_year", req.ProfitYear),
            new KeyValuePair<string, object?>("validation_result", result.Value.IsValid ? "pass" : "fail"));

        _logger.LogInformation(
            "ALLOC transfer validation completed for year {ProfitYear}: Valid={IsValid}, Summary={Summary}",
            req.ProfitYear,
            result.Value.IsValid,
            result.Value.Summary);

        return TypedResults.Ok(result.Value);
    }
}
