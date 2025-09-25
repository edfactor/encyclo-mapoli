using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;

public sealed class TerminatedEmployeesNeedingFormLetterDownloadEndpoint : ProfitSharingEndpoint<TerminatedLettersRequest, string>
{
    private readonly IAdhocTerminatedEmployeesService _adhocTerminatedEmployeesService;
    private readonly ILogger<TerminatedEmployeesNeedingFormLetterDownloadEndpoint> _logger;

    public TerminatedEmployeesNeedingFormLetterDownloadEndpoint(IAdhocTerminatedEmployeesService adhocTerminatedEmployeesService, ILogger<TerminatedEmployeesNeedingFormLetterDownloadEndpoint> logger) : base(Navigation.Constants.Unknown) //TBD
    {
        _adhocTerminatedEmployeesService = adhocTerminatedEmployeesService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("adhoc-terminated-employees-report-needing-letter/download");
        Summary(s =>
        {
            s.Summary = "Returns a text file containing a form letter to be sent to terminated employees who aren't fully vested";
            s.ExampleRequest = TerminatedLettersRequest.RequestExample();
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(TerminatedLettersRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var response = await _adhocTerminatedEmployeesService.GetFormLetterForTerminatedEmployees(req, ct);

            // Record form letter download metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "form-letter-download"),
                new("endpoint", "TerminatedEmployeesNeedingFormLetterDownloadEndpoint"),
                new("file_type", "text"),
                new("file_name", "QPROF003.txt"));

            var responseLength = response?.Length ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(responseLength,
                new("record_type", "letter-content-bytes"),
                new("endpoint", "TerminatedEmployeesNeedingFormLetterDownloadEndpoint"));

            _logger.LogInformation("Form letter download generated, file size: {FileSize} bytes (correlation: {CorrelationId})",
                responseLength, HttpContext.TraceIdentifier);

            var memoryStream = new MemoryStream();
            await using (var writer = new StreamWriter(memoryStream))
            {
                await writer.WriteAsync(response);
                await writer.FlushAsync(ct);

                memoryStream.Position = 0;

                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = "QPROF003.txt",
                    Inline = false
                };
                HttpContext.Response.Headers.Append("Content-Disposition", cd.ToString());

                // Record successful file download
                this.RecordResponseMetrics(HttpContext, _logger, response ?? string.Empty);

                await Send.StreamAsync(memoryStream, "QPROF003.txt", contentType: "text/plain", cancellation: ct);
            }
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
