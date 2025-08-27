using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;
public sealed class ProfitSharingLabelsExportEndpoint : ProfitSharingEndpoint<ProfitYearRequest, PaginatedResponseDto<ProfitSharingLabelResponse>>
{
    private readonly IPostFrozenService _postFrozenService;

    public ProfitSharingLabelsExportEndpoint(IPostFrozenService postFrozenService)
        : base(Navigation.Constants.PROFNEW)
    {
        _postFrozenService = postFrozenService;
    }
    public override void Configure()
    {
        Get("post-frozen/profit-sharing-labels/export");
        Summary(s =>
        {
            s.Summary = "Returns profit sharing labels as a file";
            s.Description = "Returns a semi-colon separated list of employee data for labels";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>()
            {
                {200, ProfitSharingLabelResponse.SampleResponse() }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(ProfitYearRequest req, CancellationToken ct)
    {
        var response = await _postFrozenService.GetProfitSharingLabelsExport(req, ct);
        var memoryStream = new MemoryStream();
        await using var writer = new StreamWriter(memoryStream);
        foreach (var line in response)
        {
            await writer.WriteLineAsync(line);
        }
        await writer.FlushAsync(ct);

        memoryStream.Position = 0;

        System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
        {
            FileName = "PROFLBL.txt",
            Inline = false
        };
        HttpContext.Response.Headers.Append("Content-Disposition", cd.ToString());

        await Send.StreamAsync(memoryStream, "PROFLBL.txt", contentType: "text/plain", cancellation: ct);
    }
}
