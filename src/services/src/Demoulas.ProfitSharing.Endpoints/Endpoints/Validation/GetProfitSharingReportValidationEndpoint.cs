using System;
using System.Collections.Generic;
using System.Text;
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
/// Endpoint for retrieving validation data for Profit Sharing Summary Reports.
/// Returns validation results for Members, Balance, and Wages checksum groups.
/// </summary>
public sealed class GetProfitSharingReportValidationEndpoint
    : ProfitSharingEndpoint<ProfitSharingReportValidationRequest, Results<Ok<ValidationResponse>, NotFound, ProblemHttpResult>>
{
    private readonly ICrossReferenceValidationService _crossReferenceValidationService;
    private readonly ILogger<GetProfitSharingReportValidationEndpoint> _logger;

    public GetProfitSharingReportValidationEndpoint(
        ICrossReferenceValidationService crossReferenceValidationService,
        ILogger<GetProfitSharingReportValidationEndpoint> logger)
        : base(Navigation.Constants.Unknown)
    {
        _crossReferenceValidationService = crossReferenceValidationService ?? throw new ArgumentNullException(nameof(crossReferenceValidationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Configure()
    {
        Get("checksum/profit-sharing-report/{profitYear}/{reportSuffix}/{useFrozenData}");
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

    public override Task<Results<Ok<ValidationResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(
        ProfitSharingReportValidationRequest req,
        CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var profitYear = req.ProfitYear;
            var reportSuffix = req.ReportSuffix;

            // Note: Year validation is handled by ProfitYearRequestValidator (2020 to current year + 1)
            // Report suffix validation (1-8) should be handled by request validator

            // Get validation data for the specified Profit Sharing Report
            var result = await _crossReferenceValidationService.ValidateProfitSharingReport(
                profitYear,
                reportSuffix,
                req.UseFrozenData,
                ct);

            if (result.IsSuccess && result.Value != null)
            {
                // Record business metrics for successful validation retrieval
                Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                    new KeyValuePair<string, object?>("operation", "profit-sharing-report-validation-retrieval"),
                    new KeyValuePair<string, object?>("endpoint", nameof(GetProfitSharingReportValidationEndpoint)),
                    new KeyValuePair<string, object?>("profit_year", profitYear),
                    new KeyValuePair<string, object?>("report_suffix", reportSuffix));

                // Record validation metrics
                var validationGroupCount = result.Value.ValidationGroups.Count;
                var validGroupCount = result.Value.ValidationGroups.Count(g => g.IsValid);

                Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.RecordCountsProcessed.Record(
                    validationGroupCount,
                    new KeyValuePair<string, object?>("record_type", "validation-groups"),
                    new KeyValuePair<string, object?>("endpoint", nameof(GetProfitSharingReportValidationEndpoint)));
            }

            return result.Match<Results<Ok<ValidationResponse>, NotFound, ProblemHttpResult>>(
                success => TypedResults.Ok(success),
                problemDetails =>
                {
                    // Check if this is a "not found" error (code 104)
                    if (result.Error?.Code == 104)
                    {
                        _logger.LogWarning(
                            "No archived checksums found for profit year {ProfitYear} and report suffix {ReportSuffix}",
                            profitYear,
                            reportSuffix);
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
