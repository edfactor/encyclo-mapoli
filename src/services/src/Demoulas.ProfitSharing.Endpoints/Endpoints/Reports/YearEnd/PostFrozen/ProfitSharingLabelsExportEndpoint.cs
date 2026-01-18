using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;

public sealed class ProfitSharingLabelsExportEndpoint : ProfitSharingEndpoint<ProfitYearRequest, Results<FileStreamHttpResult, ProblemHttpResult>>
{
    private readonly IPostFrozenService _postFrozenService;
    private readonly ILogger<ProfitSharingLabelsExportEndpoint> _logger;

    public ProfitSharingLabelsExportEndpoint(IPostFrozenService postFrozenService, ILogger<ProfitSharingLabelsExportEndpoint> logger)
        : base(Navigation.Constants.PROFNEW)
    {
        _postFrozenService = postFrozenService;
        _logger = logger;
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
                {200, ProfitSharingLabelResponse.ResponseExample() }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    protected override async Task<Results<FileStreamHttpResult, ProblemHttpResult>> HandleRequestAsync(ProfitYearRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);
        this.RecordRequestMetrics(HttpContext, _logger, req);

        try
        {
            var response = await _postFrozenService.GetProfitSharingLabelsExport(req, ct);

            // Record business operation metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "profit_sharing_labels_export"),
                new("profit_year", req.ProfitYear.ToString()));

            var lineCount = response?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(lineCount,
                new("record_type", "post-frozen-profit-sharing-labels-export"),
                new("endpoint", "ProfitSharingLabelsExportEndpoint"));

            var memoryStream = new MemoryStream();
            await using var writer = new StreamWriter(memoryStream);
            if (response != null)
            {
                foreach (var line in response)
                {
                    await writer.WriteLineAsync(line);
                }
            }
            await writer.FlushAsync(ct);

            memoryStream.Position = 0;
            var fileSizeBytes = memoryStream.Length;

            // Record file size metrics
            EndpointTelemetry.ResponseSizeBytes.Record(fileSizeBytes,
                new("file_type", "profit_labels_export"),
                new("endpoint", "ProfitSharingLabelsExportEndpoint"));

            _logger.LogInformation("Year-end post-frozen profit sharing labels export generated: {Lines} lines, {Size} bytes (correlation: {CorrelationId})",
                lineCount, fileSizeBytes, HttpContext.TraceIdentifier);

            return TypedResults.File(memoryStream, "text/plain", "PROFLBL.txt");
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
