using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Mime;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;

/// <summary>
/// Endpoint for downloading form letters for employees over age 73 with profit sharing balances.
/// Returns a text file containing form letters for employees who must take required minimum distributions (RMDs).
/// </summary>
public sealed class EmployeesWithProfitsOver73FormLetterDownloadEndpoint : ProfitSharingEndpoint<EmployeesWithProfitsOver73Request, string>
{
    private readonly IEmployeesWithProfitsOver73Service _employeesWithProfitsOver73Service;
    private readonly ILogger<EmployeesWithProfitsOver73FormLetterDownloadEndpoint> _logger;

    public EmployeesWithProfitsOver73FormLetterDownloadEndpoint(
        IEmployeesWithProfitsOver73Service employeesWithProfitsOver73Service,
        ILogger<EmployeesWithProfitsOver73FormLetterDownloadEndpoint> logger)
        : base(Navigation.Constants.AdhocProfLetter73)
    {
        _employeesWithProfitsOver73Service = employeesWithProfitsOver73Service;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("prof-letter73/download-form-letter");
        Summary(s =>
        {
            s.Summary = "Returns a text file containing form letters for employees over age 73 who must take required minimum distributions";
            s.Description = "Generates form letters (QPROF_OVER73.txt) for employees with profit sharing accounts who have reached age 73 and must comply with IRS required minimum distribution (RMD) rules. " +
                           "Optionally accepts a list of badge numbers to generate letters for specific employees only. If no badge numbers are provided, letters will be generated for all eligible employees.";
            s.ExampleRequest = new EmployeesWithProfitsOver73Request
            {
                ProfitYear = 2023,
                BadgeNumbers = new List<int> { 12345, 67890 } // Optional: specify employees, or omit for all
            };
            s.Responses[200] = "Text file (QPROF_OVER73.txt) containing form letters";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<AdhocReportsGroup>();
    }

    public override async Task HandleAsync(EmployeesWithProfitsOver73Request req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var formLetterContent = await _employeesWithProfitsOver73Service.GetEmployeesWithProfitsOver73FormLetterAsync(req, ct);

            // Record form letter download metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "form-letter-download"),
                new("endpoint", "EmployeesWithProfitsOver73FormLetterDownloadEndpoint"),
                new("report_type", "over-73-rmd"),
                new("file_type", "text"),
                new("file_name", "QPROF_OVER73.txt"));

            var responseLength = formLetterContent?.Length ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(responseLength,
                new("record_type", "letter-content-bytes"),
                new("endpoint", "EmployeesWithProfitsOver73FormLetterDownloadEndpoint"));

            _logger.LogInformation(
                "Form letter download for employees over 73 generated, file size: {FileSize} bytes (correlation: {CorrelationId})",
                responseLength,
                HttpContext.TraceIdentifier);

            var memoryStream = new MemoryStream();
            await using (var writer = new StreamWriter(memoryStream))
            {
                await writer.WriteAsync(formLetterContent);
                await writer.FlushAsync(ct);

                memoryStream.Position = 0;

                var cd = new ContentDisposition
                {
                    FileName = "QPROF_OVER73.txt",
                    Inline = false
                };
                HttpContext.Response.Headers.Append("Content-Disposition", cd.ToString());

                // Record successful file download
                this.RecordResponseMetrics(HttpContext, _logger, formLetterContent ?? string.Empty);

                await Send.StreamAsync(memoryStream, "QPROF_OVER73.txt", contentType: "text/plain", cancellation: ct);
            }
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
