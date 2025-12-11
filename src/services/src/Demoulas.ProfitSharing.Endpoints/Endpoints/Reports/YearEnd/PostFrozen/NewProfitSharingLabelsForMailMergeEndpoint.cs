using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;

public sealed class NewProfitSharingLabelsForMailMergeEndpoint : ProfitSharingEndpoint<ProfitYearRequest, PaginatedResponseDto<NewProfitSharingLabelResponse>>
{
    private readonly IPostFrozenService _postFrozenService;
    private readonly ILogger<NewProfitSharingLabelsForMailMergeEndpoint> _logger;

    public NewProfitSharingLabelsForMailMergeEndpoint(IPostFrozenService postFrozenService, ILogger<NewProfitSharingLabelsForMailMergeEndpoint> logger)
        : base(Navigation.Constants.QNEWPROFLBL)
    {
        _postFrozenService = postFrozenService;
        _logger = logger;
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
        using var activity = this.StartEndpointActivity(HttpContext);
        this.RecordRequestMetrics(HttpContext, _logger, req);

        try
        {
            var response = await _postFrozenService.GetNewProfitSharingLabelsForMailMerge(req, ct);

            // Record business operation metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "new_profit_sharing_labels_mail_merge"),
                new("profit_year", req.ProfitYear.ToString()));

            var lineCount = response?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(lineCount,
                new("record_type", "post-frozen-mail-merge-labels"),
                new("endpoint", "NewProfitSharingLabelsForMailMergeEndpoint"));

            var memoryStream = new MemoryStream();
            await using var writer = new StreamWriter(memoryStream);
            foreach (var line in response ?? Enumerable.Empty<string>())
            {
                await writer.WriteLineAsync(line);
            }
            await writer.FlushAsync(ct);

            memoryStream.Position = 0;

            // Record file size for telemetry
            EndpointTelemetry.RecordCountsProcessed.Record(memoryStream.Length,
                new("operation", "new_profit_sharing_labels_mail_merge"),
                new("metric_type", "file_size_bytes"));

            _logger.LogInformation("Year-end post-frozen new profit sharing labels for mail merge generated, {LineCount} lines, {FileSize} bytes (correlation: {CorrelationId})",
                lineCount, memoryStream.Length, HttpContext.TraceIdentifier);

            System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "NEWPROFLBL.txt",
                Inline = false
            };
            HttpContext.Response.Headers.Append("Content-Disposition", cd.ToString());

            await Send.StreamAsync(memoryStream, "NEWPROFLBL.txt", contentType: "text/plain", cancellation: ct);
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
