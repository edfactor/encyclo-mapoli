using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.YearEnd;
public sealed class CertificatesFileEndpoint : Endpoint<CerficatePrintRequest, string>
{
    private readonly ICertificateService _certificateService;

    public CertificatesFileEndpoint(ICertificateService certificateService)
    {
        _certificateService = certificateService;
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
        var response = await _certificateService.GetCertificateFile(req, ct);
        var memoryStream = new MemoryStream();
        await using var writer = new StreamWriter(memoryStream);
        await writer.WriteAsync(response);
            
        await writer.FlushAsync(ct);

        memoryStream.Position = 0;

        System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
        {
            FileName = "PAYCERT.txt",
            Inline = false
        };
        HttpContext.Response.Headers.Append("Content-Disposition", cd.ToString());

        await Send.StreamAsync(memoryStream, "PAYCERT.txt", contentType: "text/plain", cancellation: ct);
    }
}
