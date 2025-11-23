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
/// Endpoint for exporting account history reports to PDF format.
/// Generates a complete PDF document with member account activity by profit year.
/// </summary>
public sealed class ExportAccountHistoryReportPdfEndpoint : ProfitSharingEndpoint<AccountHistoryReportRequest, Results<FileStreamHttpResult, ProblemHttpResult>>
{
    private readonly IAccountHistoryReportService _accountHistoryReportService;
    private readonly ILogger<ExportAccountHistoryReportPdfEndpoint> _logger;

    public ExportAccountHistoryReportPdfEndpoint(
        IAccountHistoryReportService accountHistoryReportService,
        ILogger<ExportAccountHistoryReportPdfEndpoint> logger)
        : base(Navigation.Constants.AccountHistoryReport)
    {
        _accountHistoryReportService = accountHistoryReportService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("divorce-report/export-pdf");
        Group<AdhocReportsGroup>();
        Summary(s =>
        {
            s.Summary = "Export Account History Report to PDF";
            s.Description = "Generates and exports a complete account history report to PDF format. Includes all profit years within the specified date range with member account activity (contributions, earnings, forfeitures, withdrawals, and balances).";
            s.ExampleRequest = new AccountHistoryReportRequest
            {
                BadgeNumber = 700518,
                StartDate = new DateOnly(2017, 1, 1),
                EndDate = new DateOnly(2024, 12, 31)
            };
            s.Responses[200] = "PDF file generated successfully";
            s.Responses[400] = "Bad Request. Invalid badge number or date range.";
            s.Responses[404] = "Member not found or no account history available.";
            s.Responses[500] = "Internal Server Error.";
        });
    }

    public override async Task<Results<FileStreamHttpResult, ProblemHttpResult>> ExecuteAsync(
        AccountHistoryReportRequest req,
        CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            // Record request metrics - this endpoint accesses SSN data
            this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn", "FirstName", "LastName");

            if (req.BadgeNumber == 0)
            {
                _logger.LogWarning(
                    "Account history PDF export requested without valid badge number (correlation: {CorrelationId})",
                    HttpContext.TraceIdentifier);

                return TypedResults.Problem(
                    detail: "Invalid badge number. Badge number must be greater than 0.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            // Generate PDF using the service
            var pdfStream = await _accountHistoryReportService.GeneratePdfAsync(
                req.BadgeNumber,
                req,
                ct);

            if (pdfStream.Length == 0)
            {
                _logger.LogWarning(
                    "No account history data found for badge {BadgeNumber} (correlation: {CorrelationId})",
                    req.BadgeNumber,
                    HttpContext.TraceIdentifier);

                return TypedResults.Problem(
                    detail: $"No account history data found for member badge {req.BadgeNumber}.",
                    statusCode: StatusCodes.Status404NotFound);
            }

            // Record business metrics for PDF generation
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "account-history-report-pdf-export"),
                new("endpoint", "ExportAccountHistoryReportPdfEndpoint"),
                new("report_type", "account-history-pdf"),
                new("date_range_years", $"{(req.StartDate?.Year ?? 2017)}-{(req.EndDate?.Year ?? DateTime.Today.Year)}"));

            // Record file size metrics
            EndpointTelemetry.RecordCountsProcessed.Record((int)pdfStream.Length,
                new("record_type", "pdf-export-bytes"),
                new("endpoint", "ExportAccountHistoryReportPdfEndpoint"));

            _logger.LogInformation(
                "Account history PDF exported for badge {BadgeNumber}, file size: {FileSize} bytes (correlation: {CorrelationId})",
                req.BadgeNumber,
                pdfStream.Length,
                HttpContext.TraceIdentifier);

            // Reset stream position to beginning for output
            pdfStream.Position = 0;

            // Generate filename with member badge and date range
            var fileName = $"AccountHistory_{req.BadgeNumber}_{req.StartDate:yyyy-MM-dd}_to_{req.EndDate:yyyy-MM-dd}.pdf";

            return TypedResults.File(
                fileStream: pdfStream,
                contentType: "application/pdf",
                fileDownloadName: fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error exporting account history PDF for badge {BadgeNumber} (correlation: {CorrelationId})",
                req.BadgeNumber,
                HttpContext.TraceIdentifier);

            this.RecordException(HttpContext, _logger, ex, activity);

            return TypedResults.Problem(
                detail: "An error occurred while generating the PDF report.",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
