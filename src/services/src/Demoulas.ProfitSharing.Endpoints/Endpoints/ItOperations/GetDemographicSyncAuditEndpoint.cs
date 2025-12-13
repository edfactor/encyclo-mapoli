using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

/// <summary>
/// Gets demographic sync audit records with pagination support, ordered by creation date (descending).
/// </summary>
public class GetDemographicSyncAuditEndpoint : ProfitSharingResponseEndpoint<DemographicSyncAuditPageResponse>
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
                    new DemographicSyncAuditPageResponse
                    {
                        Records = new List<DemographicSyncAuditRecordResponse>
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
                        PageNumber = 1,
                        PageSize = 50,
                        TotalCount = 100,
                        TotalPages = 2
                    }
                }
            };
        });
        Group<ItDevOpsGroup>();
    }

    public override Task<DemographicSyncAuditPageResponse> ExecuteAsync(CancellationToken ct)
    {
        var pageNumber = Query<int?>("pageNumber") ?? 1;
        var pageSize = Query<int?>("pageSize") ?? 50;

        var request = new SortedPaginationRequestDto
        {
            Skip = (pageNumber - 1) * pageSize,
            Take = pageSize,
            SortBy = "Created",
            IsSortDescending = true
        };

        return ExecuteAsyncInternal(request, ct);
    }

    private async Task<DemographicSyncAuditPageResponse> ExecuteAsyncInternal(SortedPaginationRequestDto request, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            var result = await _service.GetDemographicSyncAuditAsync(request, ct);

            if (result.IsError)
            {
                _logger.LogError("Failed to get demographic sync audit records: {Error}", result.Error?.Description);
                return new DemographicSyncAuditPageResponse();
            }

            // Calculate pageNumber from skip and take
            int pageNumber = (request.Skip ?? 0) / (request.Take ?? 50) + 1;

            var response = new DemographicSyncAuditPageResponse
            {
                Records = result.Value!.Results
                    .Select(r => new DemographicSyncAuditRecordResponse
                    {
                        Id = r.Id,
                        BadgeNumber = r.BadgeNumber,
                        OracleHcmId = r.OracleHcmId,
                        Message = r.Message,
                        PropertyName = r.PropertyName,
                        InvalidValue = r.InvalidValue,
                        UserName = r.UserName,
                        Created = r.Created
                    })
                    .ToList(),
                PageNumber = pageNumber,
                PageSize = request.Take ?? 50,
                TotalCount = (int)result.Value!.Total,
                TotalPages = (int)Math.Ceiling((double)result.Value!.Total / (double)(request.Take ?? 50))
            };

            // Business metrics
            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "demographic-sync-audit-query"),
                    new("endpoint", "GetDemographicSyncAuditEndpoint"));

                EndpointTelemetry.RecordCountsProcessed?.Record(response.Records.Count,
                    new("record_type", "demographic-sync-audit"),
                    new("endpoint", "GetDemographicSyncAuditEndpoint"));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return response;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
