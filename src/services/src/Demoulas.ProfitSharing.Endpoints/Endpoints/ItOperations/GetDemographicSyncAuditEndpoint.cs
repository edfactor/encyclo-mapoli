using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

/// <summary>
/// Gets demographic sync audit records with pagination support, ordered by creation date (descending).
/// </summary>
public class GetDemographicSyncAuditEndpoint : ProfitSharingEndpoint<SortedPaginationRequestDto, PaginatedResponseDto<DemographicSyncAuditRecordResponse>>
{
    private readonly IOracleHcmDiagnosticsService _service;
    private readonly ILogger<GetDemographicSyncAuditEndpoint> _logger;

    public GetDemographicSyncAuditEndpoint(IOracleHcmDiagnosticsService service, ILogger<GetDemographicSyncAuditEndpoint> logger)
        : base(Navigation.Constants.OracleHcmDiagnostics)
    {
        _service = service;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("oracleHcm/audit");
        Summary(s =>
        {
            s.Summary = "Gets demographic sync audit records with pagination";
            s.Description = "Returns audit records ordered by creation date (newest first). Supports pagination with configurable page size.";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new PaginatedResponseDto<DemographicSyncAuditRecordResponse>
                    {
                        Results = new List<DemographicSyncAuditRecordResponse>
                        {
                            new()
                            {
                                Id = 1,
                                BadgeNumber = 12345,
                                OracleHcmId = 98765,
                                Message = "Successfully synced",
                                PropertyName = "FirstName",
                                InvalidValue = null,
                                UserName = "system",
                                Created = DateTimeOffset.UtcNow
                            }
                        },
                        Total = 100
                    }
                }
            };
        });
        Group<ItDevOpsGroup>();
    }

    public override Task<PaginatedResponseDto<DemographicSyncAuditRecordResponse>> ExecuteAsync(SortedPaginationRequestDto req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _service.GetDemographicSyncAuditAsync(req, ct);

            if (result.IsError)
            {
                _logger.LogError("Failed to get demographic sync audit records: {Error}", result.Error?.Description);
                return new PaginatedResponseDto<DemographicSyncAuditRecordResponse>();
            }

            var response = result.Value ?? new PaginatedResponseDto<DemographicSyncAuditRecordResponse>();

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "demographic-sync-audit-query"),
                new("endpoint", nameof(GetDemographicSyncAuditEndpoint)));

            var count = response.Results?.LongCount() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(count,
                new("record_type", "demographic-sync-audit"),
                new("endpoint", nameof(GetDemographicSyncAuditEndpoint)));

            return response;
        });
    }
}
