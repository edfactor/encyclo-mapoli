using Demoulas.ProfitSharing.Common.Contracts.Request.Validation;
using Demoulas.ProfitSharing.Common.Contracts.Response.Validation;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.CrossReference;

/// <summary>
/// Endpoint for validating specific fields of an archived report against caller-provided values.
/// Enables caller-driven validation where API consumers provide field values they're seeing
/// and want to validate against archived checksums to detect data drift.
/// </summary>
public sealed class ValidateReportChecksumEndpoint
    : ProfitSharingEndpoint<ValidateReportFieldsRequest, Results<Ok<ChecksumValidationResponse>, NotFound, ProblemHttpResult>>
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
        Post("checksum/validate-fields");
        Summary(s =>
        {
            s.Summary = "Validate report fields against archived checksums";
            s.Description = "Validates specific fields of an archived report by comparing caller-provided values " +
                            "against stored checksums. Returns detailed per-field validation results showing which fields " +
                            "match and which indicate data drift. Useful for integrity checks after data processing or reports.";
            s.RequestParam(r => r.ProfitYear, "The profit year to validate (e.g., 2024)");
            s.RequestParam(r => r.ReportType, "The report type identifier (e.g., 'PAY426N', 'YearEndBreakdown')");
            s.RequestParam(r => r.Fields, "Dictionary of field names and their current decimal values to validate");
            s.ExampleRequest = new ValidateReportFieldsRequest
            {
                ProfitYear = 2024,
                ReportType = "YearEndBreakdown",
                Fields = new Dictionary<string, decimal>
                {
                    ["TotalAmount"] = 12345.67m,
                    ["ParticipantCount"] = 100m,
                    ["AverageDistribution"] = 123.45m
                }
            };
            s.Response<ChecksumValidationResponse>(200, "Validation completed. Check IsValid and FieldResults for per-field details.");
            s.Response(404, "No archived report found for the specified profit year and report type");
            s.Response(400, "Invalid request parameters");
            s.Response(403, "Forbidden - Requires IT-DevOps or System-Administrator role");
        });
        Group<ValidationGroup>();
    }

    public override async Task<Results<Ok<ChecksumValidationResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(
        ValidateReportFieldsRequest req,
        CancellationToken ct)
    {
        // Ensure Fields dictionary is not null (required field should be validated by FastEndpoints)
        if (req.Fields == null)
        {
            _logger.LogWarning("ValidateReportFieldsRequest received with null Fields dictionary");
            return TypedResults.Problem(
                title: "Invalid Request",
                detail: "Fields dictionary is required",
                statusCode: 400);
        }

        _logger.LogInformation(
            "Validating {FieldCount} field(s) for report {ReportType} year {ProfitYear}",
            req.Fields.Count,
            req.ReportType,
            req.ProfitYear);

        var result = await _validationService.ValidateReportFieldsAsync(
            req.ProfitYear,
            req.ReportType,
            req.Fields,
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

            // Log validation result with field counts
            _logger.LogInformation(
                "Checksum validation {Result} for {ReportType} year {ProfitYear}: {MatchedCount} matched, {MismatchedCount} mismatched out of {TotalCount} fields",
                response.IsValid ? "PASSED" : "FAILED",
                req.ReportType,
                req.ProfitYear,
                response.FieldResults.Count(f => f.Value.Matches),
                response.MismatchedFields.Count,
                response.FieldResults.Count);

            // If validation failed (data drift detected), log details
            if (!response.IsValid)
            {
                _logger.LogWarning(
                    "Data integrity issue detected: {ReportType} year {ProfitYear} - Mismatched fields: {MismatchedFields}",
                    req.ReportType,
                    req.ProfitYear,
                    string.Join(", ", response.MismatchedFields));
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

        // Map Result<T> to HTTP response
        // NotFound (404) if no archived report exists
        // Problem (500) for other errors
        return result.Match<Results<Ok<ChecksumValidationResponse>, NotFound, ProblemHttpResult>>(
            v => TypedResults.Ok(v),
            pd =>
            {
                // Check if this is a "not found" error (code 104)
                if (result.Error?.Code == 104)
                {
                    return TypedResults.NotFound();
                }
                return TypedResults.Problem(pd.Detail);
            });
    }
}
