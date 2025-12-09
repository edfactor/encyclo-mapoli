using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;

/// <summary>
/// Endpoint for downloading account history report as a PDF file.
/// PS-2284: Ensure audit tracking for Account History download.
/// This endpoint generates a PDF for the specified member and tracks the download in the audit log.
/// </summary>
public sealed class AccountHistoryPdfDownloadEndpoint : ProfitSharingEndpoint<AccountHistoryReportRequest, Results<FileStreamHttpResult, BadRequest<object>, ProblemHttpResult>>
{
    private readonly IAccountHistoryReportService _accountHistoryReportService;
    private readonly ILogger<AccountHistoryPdfDownloadEndpoint> _logger;

    public AccountHistoryPdfDownloadEndpoint(
        IAccountHistoryReportService accountHistoryReportService,
        ILogger<AccountHistoryPdfDownloadEndpoint> logger)
        : base(Navigation.Constants.AccountHistoryReport)
    {
        _accountHistoryReportService = accountHistoryReportService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("account-history-pdf-download");
        Group<AdhocReportsGroup>();
        Summary(s =>
        {
            s.Summary = "Download Account History Report as PDF";
            s.Description = "Generates and downloads the account history report as a PDF file. Tracks download in audit log for compliance and security purposes (PS-2284).";
            s.ExampleRequest = new AccountHistoryReportRequest
            {
                BadgeNumber = 700518,
                StartDate = new DateOnly(2017, 1, 1),
                EndDate = new DateOnly(2024, 12, 31)
            };
            s.Responses[200] = "PDF file generated successfully and ready for download";
            s.Responses[400] = "Bad Request. Invalid badge number or date range.";
            s.Responses[404] = "No account history data found for the specified member.";
            s.Responses[500] = "Internal Server Error.";
        });
    }

    public override async Task<Results<FileStreamHttpResult, BadRequest<object>, ProblemHttpResult>> ExecuteAsync(
        AccountHistoryReportRequest req,
        CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            // Record request metrics - this endpoint accesses SSN data
            this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn");

            // Validate badge number
            if (req.BadgeNumber == 0)
            {
                _logger.LogWarning(
                    "Account history PDF download requested without valid badge number (correlation: {CorrelationId})",
                    HttpContext.TraceIdentifier);

                return TypedResults.BadRequest((object)"BadgeNumber is required and must be greater than zero");
            }

            _logger.LogInformation(
                "Starting Account History PDF download for badge {BadgeNumber}, profit year range: {StartYear} - {EndYear} (correlation: {CorrelationId})",
                req.BadgeNumber,
                req.StartDate?.Year ?? DateTime.Now.Year - 3,
                req.EndDate?.Year ?? DateTime.Now.Year,
                HttpContext.TraceIdentifier);

            // Generate PDF - includes audit logging internally (PS-2284)
            var pdfStream = await _accountHistoryReportService.GeneratePdfAsync(
                req.BadgeNumber,
                req,
                ct);

            // Record business metrics for PDF download
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "account-history-pdf-download"),
                new("endpoint", "AccountHistoryPdfDownloadEndpoint"),
                new("report_type", "account-history-pdf"),
                new("date_range_years", $"{(req.StartDate?.Year ?? 2017)}-{(req.EndDate?.Year ?? DateTime.Today.Year)}"));

            EndpointTelemetry.SensitiveFieldAccessTotal.Add(1,
                new("field", "Ssn"),
                new("endpoint", "AccountHistoryPdfDownloadEndpoint"));

            _logger.LogInformation(
                "Successfully generated Account History PDF for badge {BadgeNumber}, file size: {FileSizeKb} KB (correlation: {CorrelationId})",
                req.BadgeNumber,
                pdfStream.Length / 1024,
                HttpContext.TraceIdentifier);

            // Return PDF as file stream with appropriate headers
            var fileName = $"AccountHistory_Badge{req.BadgeNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return TypedResults.File(
                pdfStream,
                contentType: "application/pdf",
                fileDownloadName: fileName);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                ex,
                "No account history data found for badge {BadgeNumber} (correlation: {CorrelationId})",
                req.BadgeNumber,
                HttpContext.TraceIdentifier);

            this.RecordException(HttpContext, _logger, ex, activity);
            return TypedResults.Problem(
                detail: $"No account history found for badge {req.BadgeNumber}",
                statusCode: StatusCodes.Status404NotFound,
                title: "Account History Not Found");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error generating Account History PDF for badge {BadgeNumber} (correlation: {CorrelationId})",
                req.BadgeNumber,
                HttpContext.TraceIdentifier);

            this.RecordException(HttpContext, _logger, ex, activity);
            return TypedResults.Problem(
                detail: $"Failed to generate PDF: {ex.Message}",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "PDF Generation Failed");
        }
    }
}
