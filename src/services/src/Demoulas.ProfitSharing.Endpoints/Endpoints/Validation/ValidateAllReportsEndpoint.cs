using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Validation;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Validation;

/// <summary>
/// Endpoint for batch validation of archived report checksums.
/// Validates all archived reports or filters by profit year.
/// </summary>
public sealed class ValidateAllReportsEndpoint
    : ProfitSharingEndpoint<ValidateAllReportsRequest, Results<Ok<List<ChecksumValidationResponse>>, ProblemHttpResult>>
{
    private readonly IChecksumValidationService _validationService;
    private readonly ILogger<ValidateAllReportsEndpoint> _logger;

    public ValidateAllReportsEndpoint(
        IChecksumValidationService validationService,
        ILogger<ValidateAllReportsEndpoint> logger)
        : base(Navigation.Constants.Unknown)
    {
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Post("checksum/validate-all");
        Summary(s =>
        {
            s.Summary = "Batch validate archived report checksums";
            s.Description = "Validates multiple archived reports by comparing stored checksums with fresh calculations. " +
                            "Can filter by profit year or validate all archived reports. " +
                            "Used for periodic integrity audits and detecting data drift across multiple reports.";
            s.RequestParam(r => r.ProfitYear, "Optional profit year filter. If null, validates all archived reports.");
            s.Response<List<ChecksumValidationResponse>>(200, "Batch validation completed. Check individual IsValid properties.");
            s.Response(400, "Invalid request parameters");
            s.Response(403, "Forbidden - Requires IT-DevOps or System-Administrator role");
        });
        Group<ValidationGroup>();
    }

    public override async Task<Results<Ok<List<ChecksumValidationResponse>>, ProblemHttpResult>> ExecuteAsync(
        ValidateAllReportsRequest req,
        CancellationToken ct)
    {
        _logger.LogInformation(
            "Starting batch validation for {Scope}",
            req.ProfitYear.HasValue
                ? $"profit year {req.ProfitYear.Value}"
                : "all profit years");

        var result = await _validationService.ValidateAllReportsAsync(req.ProfitYear, ct);

        if (result.IsSuccess)
        {
            var responses = result.Value!;

            // Calculate summary statistics
            int totalCount = responses.Count;
            int validCount = responses.Count(r => r.IsValid);
            int invalidCount = totalCount - validCount;

            // Record business metrics
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "batch-checksum-validation"),
                new KeyValuePair<string, object?>("endpoint.category", "validation"),
                new KeyValuePair<string, object?>("profit_year", req.ProfitYear?.ToString() ?? "all"),
                new KeyValuePair<string, object?>("total_count", totalCount),
                new KeyValuePair<string, object?>("valid_count", validCount),
                new KeyValuePair<string, object?>("invalid_count", invalidCount));

            // Record counts processed
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.RecordCountsProcessed.Record(totalCount,
                new KeyValuePair<string, object?>("record_type", "checksum-validations"),
                new KeyValuePair<string, object?>("endpoint.category", "validation"));

            // Log summary
            _logger.LogInformation(
                "Batch validation complete: {TotalCount} reports validated, {ValidCount} valid, {InvalidCount} invalid",
                totalCount,
                validCount,
                invalidCount);

            // Log details for any failed validations (data integrity issues)
            if (invalidCount > 0)
            {
                foreach (var invalid in responses.Where(r => !r.IsValid))
                {
                    _logger.LogWarning(
                        "Data integrity issue: {ReportType} year {ProfitYear} - {Message}",
                        invalid.ReportType,
                        invalid.ProfitYear,
                        invalid.Message);
                }
            }
        }
        else
        {
            // Handle errors
            _logger.LogError(
                "Batch validation failed: {Error}",
                result.Error?.Description ?? "Unknown error");
        }

        // Convert result to HTTP response (no NotFound needed - returns empty list if no reports)
        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value!);
        }

        return TypedResults.Problem(result.Error?.Description ?? "Batch validation failed");
    }
}
