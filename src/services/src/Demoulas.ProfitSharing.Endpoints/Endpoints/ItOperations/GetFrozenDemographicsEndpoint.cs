using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Microsoft.Extensions.Logging;
using FastEndpoints;

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
        Group<ItDevOpsAllUsersGroup>();
    }

    public override Task<PaginatedResponseDto<FrozenStateResponse>> ExecuteAsync(SortedPaginationRequestDto req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var response = await _frozenService.GetFrozenDemographics(req, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "frozen-demographics-query"),
                new("endpoint", "GetFrozenDemographicsEndpoint"));

            var resultCount = response?.Total ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "frozen-demographics"),
                new("endpoint", "GetFrozenDemographicsEndpoint"));

            _logger.LogInformation("Frozen demographics query completed, returned {ResultCount} records (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            return response ?? new PaginatedResponseDto<FrozenStateResponse>();
        });
    }
}
