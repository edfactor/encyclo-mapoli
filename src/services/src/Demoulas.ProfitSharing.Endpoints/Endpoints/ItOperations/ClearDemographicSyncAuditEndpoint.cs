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
/// Clears all records from the DEMOGRAPHIC_SYNC_AUDIT table.
/// Returns the count of deleted records.
/// </summary>
public class ClearDemographicSyncAuditEndpoint : ProfitSharingResponseEndpoint<ClearAuditResponse>
{
    private readonly IOracleHcmDiagnosticsService _service;
    private readonly ILogger<ClearDemographicSyncAuditEndpoint> _logger;

    public ClearDemographicSyncAuditEndpoint(IOracleHcmDiagnosticsService service, ILogger<ClearDemographicSyncAuditEndpoint> logger)
        : base(Navigation.Constants.OracleHcmDiagnostics)
    {
        _service = service;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("oracleHcm/audit/clear");
        Summary(s =>
        {
            s.Summary = "Clears all demographic sync audit records";
            s.Description = "Removes all records from the DEMOGRAPHIC_SYNC_AUDIT table. Returns the count of deleted records.";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ClearAuditResponse
                    {
                        DeletedCount = 1247
                    }
                }
            };
        });
        Group<ItDevOpsGroup>();
    }

    public override Task<ClearAuditResponse> ExecuteAsync(CancellationToken ct)
    {
        return ExecuteAsyncInternal(ct);
    }

    private async Task<ClearAuditResponse> ExecuteAsyncInternal(CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            var result = await _service.ClearDemographicSyncAuditAsync(ct);

            if (result.IsError)
            {
                _logger.LogError("Failed to clear demographic sync audit records: {Error}", result.Error?.Description);
                return new ClearAuditResponse { DeletedCount = 0 };
            }

            var response = new ClearAuditResponse
            {
                DeletedCount = result.Value!
            };

            // Business metrics - record the clear operation
            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "clear-demographic-sync-audit"),
                    new("endpoint", "ClearDemographicSyncAuditEndpoint"));

                EndpointTelemetry.RecordCountsProcessed?.Record(response.DeletedCount,
                    new("record_type", "audit-records-deleted"),
                    new("endpoint", "ClearDemographicSyncAuditEndpoint"));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            _logger.LogInformation("Cleared {DeletedCount} demographic sync audit records (correlation: {CorrelationId})",
                response.DeletedCount, HttpContext.TraceIdentifier);

            return response;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
