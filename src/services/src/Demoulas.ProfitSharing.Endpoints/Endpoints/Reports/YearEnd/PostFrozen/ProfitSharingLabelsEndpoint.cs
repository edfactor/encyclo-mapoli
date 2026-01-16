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
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;

public sealed class ProfitSharingLabelsEndpoint : ProfitSharingEndpoint<FrozenProfitYearRequest, PaginatedResponseDto<ProfitSharingLabelResponse>>
{
    private readonly IPostFrozenService _postFrozenService;
    private readonly ILogger<ProfitSharingLabelsEndpoint> _logger;

    public ProfitSharingLabelsEndpoint(IPostFrozenService postFrozenService, ILogger<ProfitSharingLabelsEndpoint> logger)
        : base(Navigation.Constants.PROFALL)
    {
        _postFrozenService = postFrozenService;
        _logger = logger;
    }
    public override void Configure()
    {
        Get("post-frozen/profit-sharing-labels");
        Summary(s =>
        {
            s.Summary = "Returns data for the profit sharing labels";
            s.Description = "Returns the JSON needed for the report showing which labels will be produced";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>()
            {
                {200, ProfitSharingLabelResponse.ResponseExample() }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    protected override async Task<PaginatedResponseDto<ProfitSharingLabelResponse>> HandleRequestAsync(FrozenProfitYearRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);
        this.RecordRequestMetrics(HttpContext, _logger, req);

        try
        {
            var response = await _postFrozenService.GetProfitSharingLabels(req, ct);

            // Record business operation metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "profit_sharing_labels"),
                new("profit_year", req.ProfitYear.ToString()));

            var resultCount = response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "post-frozen-profit-sharing-labels"),
                new("endpoint", "ProfitSharingLabelsEndpoint"));

            _logger.LogInformation("Year-end post-frozen profit sharing labels generated, returned {Count} label records (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            if (response != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, response);
                return response;
            }

            var emptyResponse = new PaginatedResponseDto<ProfitSharingLabelResponse> { Results = [] };
            this.RecordResponseMetrics(HttpContext, _logger, emptyResponse);
            return emptyResponse;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
