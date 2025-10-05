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
/// Endpoint for validating a single archived report's checksum against current data.
/// Detects data drift or corruption in archived reports.
/// </summary>
public sealed class ValidateReportChecksumEndpoint
    : ProfitSharingEndpoint<ValidateReportChecksumRequest, Results<Ok<ChecksumValidationResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IChecksumValidationService _validationService;
    private readonly ILogger<ValidateReportChecksumEndpoint> _logger;

    public ValidateReportChecksumEndpoint(
        IChecksumValidationService validationService,
        ILogger<ValidateReportChecksumEndpoint> logger)
        : base(Navigation.Constants.Unknown)
    {
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Post("checksum/validate");
        Summary(s =>
        {
            s.Summary = "Validate archived report checksum";
            s.Description = "Validates an archived report by comparing its stored checksum with a fresh calculation. " +
                            "Used to detect data drift, corruption, or integrity issues in archived reports.";
            s.RequestParam(r => r.ProfitYear, "The profit year to validate (e.g., 2024)");
            s.RequestParam(r => r.ReportType, "The report type identifier (e.g., 'PAY426N', 'YearEndBreakdown')");
            s.Response<ChecksumValidationResponse>(200, "Validation completed successfully. Check IsValid property for result.");
            s.Response(404, "No archived report found for the specified profit year and report type");
            s.Response(400, "Invalid request parameters");
            s.Response(403, "Forbidden - Requires IT-DevOps or System-Administrator role");
        });
        Group<ValidationGroup>();
    }

    public override async Task<Results<Ok<ChecksumValidationResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(
        ValidateReportChecksumRequest req,
        CancellationToken ct)
    {
        _logger.LogInformation(
            "Validating checksum for report {ReportType} year {ProfitYear}",
            req.ReportType,
            req.ProfitYear);

        var result = await _validationService.ValidateReportChecksumAsync(
            req.ProfitYear,
            req.ReportType,
            ct);

        // Record business metrics
        Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new KeyValuePair<string, object?>("operation", "checksum-validation"),
            new KeyValuePair<string, object?>("endpoint.category", "validation"),
            new KeyValuePair<string, object?>("report_type", req.ReportType),
            new KeyValuePair<string, object?>("profit_year", req.ProfitYear),
            new KeyValuePair<string, object?>("validation_result", result.IsSuccess ? "success" : "failure"));

        if (result.IsSuccess)
        {
            var response = result.Value!;

            // Log validation result
            _logger.LogInformation(
                "Checksum validation {Result} for {ReportType} year {ProfitYear} (IsValid: {IsValid})",
                response.IsValid ? "PASSED" : "FAILED",
                req.ReportType,
                req.ProfitYear,
                response.IsValid);

            // If validation failed (data drift detected), log as warning
            if (!response.IsValid)
            {
                _logger.LogWarning(
                    "Data integrity issue detected: {ReportType} year {ProfitYear} - {Message}",
                    req.ReportType,
                    req.ProfitYear,
                    response.Message);
            }
        }
        else
        {
            // Handle errors (e.g., no archived report found)
            _logger.LogWarning(
                "Checksum validation failed for {ReportType} year {ProfitYear}: {Error}",
                req.ReportType,
                req.ProfitYear,
                result.Error?.Description ?? "Unknown error");
        }

        return result.ToHttpResult(Error.EntityNotFound("Archived report"));
    }
}
