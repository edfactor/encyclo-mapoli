using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

/// <summary>
/// Clears all records from the DEMOGRAPHIC_SYNC_AUDIT table.
/// Returns the count of deleted records.
/// </summary>
public class ClearDemographicSyncAuditEndpoint
    : ProfitSharingEndpoint<EmptyRequest, Results<Ok<ClearAuditResponse>, NotFound, ProblemHttpResult>>
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
        Post("oracle-hcm/audit/clear");
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
        Group<AdministrationGroup>();
    }

    public override Task<Results<Ok<ClearAuditResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _service.ClearDemographicSyncAuditAsync(ct);

            var responseResult = result.Match(
                v => Result<ClearAuditResponse>.Success(new ClearAuditResponse { DeletedCount = v }),
                _ => Result<ClearAuditResponse>.Failure(result.Error!));

            if (responseResult.IsSuccess)
            {
                EndpointTelemetry.BusinessOperationsTotal.Add(1,
                    new("operation", "clear-demographic-sync-audit"),
                    new("endpoint", nameof(ClearDemographicSyncAuditEndpoint)));

                EndpointTelemetry.RecordCountsProcessed.Record(responseResult.Value!.DeletedCount,
                    new("record_type", "audit-records-deleted"),
                    new("endpoint", nameof(ClearDemographicSyncAuditEndpoint)));

                _logger.LogInformation("Cleared {DeletedCount} demographic sync audit records (correlation: {CorrelationId})",
                    responseResult.Value!.DeletedCount,
                    HttpContext.TraceIdentifier);
            }

            return responseResult.ToHttpResult();
        });
    }
}
