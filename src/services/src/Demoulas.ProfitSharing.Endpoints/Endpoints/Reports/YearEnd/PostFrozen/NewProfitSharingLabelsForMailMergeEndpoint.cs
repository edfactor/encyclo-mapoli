using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;
public sealed class NewProfitSharingLabelsForMailMergeEndpoint : Endpoint<ProfitYearRequest, PaginatedResponseDto<NewProfitSharingLabelResponse>>
{
    private readonly IPostFrozenService _postFrozenService;

    public NewProfitSharingLabelsForMailMergeEndpoint(IPostFrozenService postFrozenService)
    {
        _postFrozenService = postFrozenService;
    }
    public override void Configure()
    {
        Get("post-frozen/new-profit-sharing-labels/download");
        Summary(s =>
        {
            s.Summary = "Returns the new profit sharing labels as a file";
            s.Description = "Returns either the JSON needed for the report showing which labels will be produced";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>()
            {
                {200, NewProfitSharingLabelResponse.SampleResponse() }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(ProfitYearRequest req, CancellationToken ct)
    {
        var response = await _postFrozenService.GetNewProfitSharingLabelsForMailMerge(req, ct);
        var memoryStream = new MemoryStream();
        await using (var writer = new StreamWriter(memoryStream))
        {
            foreach (var line in response)
            {
                await writer.WriteLineAsync(line);
            }
            await writer.FlushAsync(ct);

            memoryStream.Position = 0;

            System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "NEWPROFLBL.txt",
                Inline = false
            };
            HttpContext.Response.Headers.Append("Content-Disposition", cd.ToString());

            await Send.StreamAsync(memoryStream, "NEWPROFLBL.txt", contentType: "text/plain", cancellation: ct);
        }
    }
}
