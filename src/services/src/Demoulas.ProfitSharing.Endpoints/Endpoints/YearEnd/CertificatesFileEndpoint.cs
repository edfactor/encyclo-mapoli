using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.YearEnd;

public sealed class CertificatesFileEndpoint : ProfitSharingEndpoint<CerficatePrintRequest, string>
{
    private readonly ICertificateService _certificateService;
    private readonly ILogger<CertificatesFileEndpoint> _logger;

    public CertificatesFileEndpoint(ICertificateService certificateService, ILogger<CertificatesFileEndpoint> logger)
        : base(Navigation.Constants.PrintProfitCerts)
    {
        _certificateService = certificateService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("post-frozen/certificates/download");
        Summary(s =>
        {
            s.Summary = "Returns the core certificate file to be used with pre-printed form letters";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(CerficatePrintRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            // Record request metrics
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _certificateService.GetCertificateFile(req, ct);

            if (!result.IsSuccess)
            {
                // Validation error (e.g., missing annuity rates) - return 400 BadRequest
                if (result.Error?.ValidationErrors?.Count > 0)
                {
                    _logger.LogWarning("Certificate file generation failed validation for profit year {ProfitYear}: {ErrorMessage} (correlation: {CorrelationId})",
                        req.ProfitYear, result.Error.Description, HttpContext.TraceIdentifier);

                    foreach (var error in result.Error.ValidationErrors)
                    {
                        foreach (var message in error.Value)
                        {
                            AddError(error.Key, message);
                        }
                    }
                    ThrowError(result.Error.Description, 400);
                    return;
                }

                // Service error - return 500
                _logger.LogError("Certificate file generation failed for profit year {ProfitYear}: {ErrorMessage} (correlation: {CorrelationId})",
                    req.ProfitYear, result.Error?.Description ?? "Unknown error", HttpContext.TraceIdentifier);

                AddError("CertificateGeneration", result.Error?.Description ?? "Failed to generate certificate file.");
                ThrowError(result.Error?.Description ?? "Failed to generate certificate file.", 500);
                return;
            }

            var response = result.Value!;
            var memoryStream = new MemoryStream();
            await using var writer = new StreamWriter(memoryStream);
            await writer.WriteAsync(response);

            await writer.FlushAsync(ct);

            memoryStream.Position = 0;
            var fileSize = memoryStream.Length;

            // Record business metrics - file download
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "certificate-file-download"),
                new KeyValuePair<string, object?>("endpoint.category", "year-end"));

            // Record file size
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.RequestSizeBytes.Record(fileSize,
                new KeyValuePair<string, object?>("direction", "response"),
                new KeyValuePair<string, object?>("endpoint.category", "year-end"));

            // Log certificate file generation
            _logger.LogInformation("Certificate file generated for profit year {ProfitYear}, file size: {FileSize} bytes (correlation: {CorrelationId})",
                req.ProfitYear, fileSize, HttpContext.TraceIdentifier);

            System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition { FileName = "PAYCERT.txt", Inline = false };
            HttpContext.Response.Headers.Append("Content-Disposition", cd.ToString());

            await Send.StreamAsync(memoryStream, "PAYCERT.txt", contentType: "text/plain", cancellation: ct);

            // Record successful file download
            this.RecordResponseMetrics(HttpContext, _logger, new { FileSize = fileSize, FileName = "PAYCERT.txt" }, isSuccess: true);
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
