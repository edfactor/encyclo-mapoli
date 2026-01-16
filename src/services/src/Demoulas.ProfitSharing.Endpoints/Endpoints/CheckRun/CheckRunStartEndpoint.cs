using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.CheckRun;
using Demoulas.ProfitSharing.Common.Interfaces.CheckRun;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.CheckRun;

/// <summary>
/// Endpoint to start a new profit sharing check run.
/// Generates a printer-ready file for explicit distribution ids.
/// </summary>
public sealed class CheckRunStartEndpoint : ProfitSharingEndpoint<CheckRunStartRequest, string>
{
    private readonly ICheckRunPrintFileService _printFileService;
    private readonly ILogger<CheckRunStartEndpoint> _logger;

    public CheckRunStartEndpoint(
        ICheckRunPrintFileService printFileService,
        ILogger<CheckRunStartEndpoint> logger)
        : base(Navigation.Constants.CheckRun)
    {
        _printFileService = printFileService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("start");
        Summary(s =>
        {
            s.Summary = "Start a new profit sharing check run";
            s.Description = "Generates a printer-ready file for the specified DistributionIds (Xerox DJDE or standard output).";
            s.ExampleRequest = CheckRunStartRequest.RequestExample();
            s.Responses[400] = "Bad Request. Invalid input parameters or validation errors.";
            s.Responses[500] = "Internal Server Error. Workflow execution failed.";
        });
        Group<CheckRunGroup>();
    }

    protected override async Task<string> HandleRequestAsync(CheckRunStartRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _printFileService.GenerateAsync(req, ct);

            if (!result.IsSuccess)
            {
                if (result.Error?.ValidationErrors?.Count > 0)
                {
                    foreach (var error in result.Error.ValidationErrors)
                    {
                        foreach (var message in error.Value)
                        {
                            AddError(error.Key, message);
                        }
                    }

                    ThrowError(result.Error.Description, 400);
                    return string.Empty;
                }

                AddError("CheckRun", result.Error?.Description ?? "Failed to generate check run file.");
                ThrowError(result.Error?.Description ?? "Failed to generate check run file.", 500);
                return string.Empty;
            }

            var response = result.Value!;
            var memoryStream = new MemoryStream();
            await using var writer = new StreamWriter(memoryStream);
            await writer.WriteAsync(response.Content);
            await writer.FlushAsync(ct);
            memoryStream.Position = 0;

            var fileSize = memoryStream.Length;

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "check-run-print-file"),
                new("endpoint", nameof(CheckRunStartEndpoint)),
                new("profit_year", req.ProfitYear.ToString()),
                new("printer_type", req.PrinterType.ToString()));

            EndpointTelemetry.RequestSizeBytes.Record(fileSize,
                new("direction", "response"),
                new("endpoint.category", "check-run"));

            _logger.LogInformation(
                "Generated check run print file. ProfitYear={ProfitYear} RunId={RunId} CheckCount={CheckCount} FileName={FileName} FileSize={FileSize} (correlation: {CorrelationId})",
                req.ProfitYear,
                response.RunId,
                response.CheckCount,
                response.FileName,
                fileSize,
                HttpContext.TraceIdentifier);

            System.Net.Mime.ContentDisposition cd = new() { FileName = response.FileName, Inline = false };
            HttpContext.Response.Headers.Append("Content-Disposition", cd.ToString());

            await Send.StreamAsync(memoryStream, response.FileName, contentType: response.ContentType, cancellation: ct);

            this.RecordResponseMetrics(HttpContext, _logger, new { FileSize = fileSize, response.FileName }, isSuccess: true);
            return string.Empty;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
