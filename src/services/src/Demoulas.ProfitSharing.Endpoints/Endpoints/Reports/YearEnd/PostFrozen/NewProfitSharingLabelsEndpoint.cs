using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;

public class NewProfitSharingLabelsEndpoint : ProfitSharingEndpoint<ProfitYearRequest, PaginatedResponseDto<NewProfitSharingLabelResponse>>
{
    private readonly IPostFrozenService _postFrozenService;
    private readonly ILogger<NewProfitSharingLabelsEndpoint> _logger;

    public NewProfitSharingLabelsEndpoint(IPostFrozenService postFrozenService, ILogger<NewProfitSharingLabelsEndpoint> logger)
        : base(Navigation.Constants.QNEWPROFLBL)
    {
        _postFrozenService = postFrozenService;
        _logger = logger;
    }
    public override void Configure()
    {
        Get("post-frozen/new-profit-sharing-labels");
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

    public sealed override async Task HandleAsync(ProfitYearRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);
        this.RecordRequestMetrics(HttpContext, _logger, req);

        try
        {
            var response = await _postFrozenService.GetNewProfitSharingLabels(req, ct);

            // Record business operation metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "new_profit_sharing_labels"),
                new("profit_year", req.ProfitYear.ToString()));

            var resultCount = response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "post-frozen-new-labels"),
                new("endpoint", "NewProfitSharingLabelsEndpoint"));

            _logger.LogInformation("Year-end post-frozen new profit sharing labels generated, returned {Count} label records (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            if (response != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, response);
                await Send.OkAsync(response, ct);
            }
            else
            {
                var emptyResponse = new PaginatedResponseDto<NewProfitSharingLabelResponse> { Results = [] };
                this.RecordResponseMetrics(HttpContext, _logger, emptyResponse);
                await Send.OkAsync(emptyResponse, ct);
            }
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
