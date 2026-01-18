using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

public class GetFrozenDemographicsEndpoint : ProfitSharingEndpoint<SortedPaginationRequestDto, PaginatedResponseDto<FrozenStateResponse>>
{
    private readonly IFrozenService _frozenService;
    private readonly ILogger<GetFrozenDemographicsEndpoint> _logger;

    public GetFrozenDemographicsEndpoint(IFrozenService frozenService, ILogger<GetFrozenDemographicsEndpoint> logger)
        : base(Navigation.Constants.DemographicFreeze)
    {
        _frozenService = frozenService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("frozen");
        Summary(s =>
        {
            s.Summary = "Gets frozen demographic meta data";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new PaginatedResponseDto<FrozenStateResponse>
                    {
                        Results = new List<FrozenStateResponse>
                        {
                            new FrozenStateResponse
                            {
                                Id = 2,
                                ProfitYear = Convert.ToInt16(DateTime.Now.Year),
                                FrozenBy = "Somebody",
                                AsOfDateTime = DateTime.Today,
                                IsActive = false,
                            }
                        }
                    }
                }
            };
        });
        Group<ItDevOpsGroup>();
    }

    protected override Task<PaginatedResponseDto<FrozenStateResponse>> HandleRequestAsync(
        SortedPaginationRequestDto req,
        CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var response = await _frozenService.GetFrozenDemographicsAsync(req, ct);

            var cacheStatus = HttpContext.Response.Headers.ContainsKey("X-Cache") ? "hit" : "miss";
            if (cacheStatus == "hit")
            {
                EndpointTelemetry.CacheHitsTotal.Add(1,
                    new("cache_type", "output-cache"),
                    new("endpoint", nameof(GetFrozenDemographicsEndpoint)));
            }
            else
            {
                EndpointTelemetry.CacheMissesTotal.Add(1,
                    new("cache_type", "output-cache"),
                    new("endpoint", nameof(GetFrozenDemographicsEndpoint)));
            }

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "frozen-demographics-query"),
                new("endpoint", nameof(GetFrozenDemographicsEndpoint)));

            var resultCount = response?.Total ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "frozen-demographics"),
                new("endpoint", nameof(GetFrozenDemographicsEndpoint)));

            _logger.LogInformation("Frozen demographics query completed, returned {ResultCount} records, cache status: {CacheStatus} (correlation: {CorrelationId})",
                resultCount, cacheStatus, HttpContext.TraceIdentifier);

            return response ?? new PaginatedResponseDto<FrozenStateResponse>();
        });
    }
}
