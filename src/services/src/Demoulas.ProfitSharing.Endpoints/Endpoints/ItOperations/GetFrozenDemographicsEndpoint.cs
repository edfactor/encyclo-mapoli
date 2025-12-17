using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

public class GetFrozenDemographicsEndpoint : ProfitSharingEndpoint<SortedPaginationRequestDto, PaginatedResponseDto<FrozenStateResponse>>
{
    private readonly IFrozenService _frozenService;
    private readonly ILogger<GetFrozenDemographicsEndpoint> _logger;

    public GetFrozenDemographicsEndpoint(IFrozenService frozenService, ILogger<GetFrozenDemographicsEndpoint> logger) : base(Navigation.Constants.DemographicFreeze)
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

        // Output caching: Frozen demographics are immutable snapshots - excellent caching candidate  
        // Cache disabled in test environments to ensure test data freshness
        if (!Env.IsTestEnvironment())
        {
            TimeSpan cacheDuration = TimeSpan.FromMinutes(15); // Long duration - frozen data never changes
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override Task<PaginatedResponseDto<FrozenStateResponse>> ExecuteAsync(SortedPaginationRequestDto req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var response = await _frozenService.GetFrozenDemographics(req, ct);

            // Record cache metrics
            var cacheStatus = HttpContext.Response.Headers.ContainsKey("X-Cache") ? "hit" : "miss";
            if (cacheStatus == "hit")
            {
                EndpointTelemetry.CacheHitsTotal.Add(1,
                    new("cache_type", "output-cache"),
                    new("endpoint", "GetFrozenDemographicsEndpoint"));
            }
            else
            {
                EndpointTelemetry.CacheMissesTotal.Add(1,
                    new("cache_type", "output-cache"),
                    new("endpoint", "GetFrozenDemographicsEndpoint"));
            }

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "frozen-demographics-query"),
                new("endpoint", "GetFrozenDemographicsEndpoint"));

            var resultCount = response?.Total ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "frozen-demographics"),
                new("endpoint", "GetFrozenDemographicsEndpoint"));

            _logger.LogInformation("Frozen demographics query completed, returned {ResultCount} records, cache status: {CacheStatus} (correlation: {CorrelationId})",
                resultCount, cacheStatus, HttpContext.TraceIdentifier);

            return response ?? new PaginatedResponseDto<FrozenStateResponse>();
        });
    }
}
