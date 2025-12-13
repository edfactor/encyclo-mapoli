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
/// Gets OracleHcm sync metadata including the most recent create and modify timestamps
/// from Demographic and PayProfit tables.
/// </summary>
public class GetOracleHcmSyncMetadataEndpoint : ProfitSharingResponseEndpoint<OracleHcmSyncMetadataResponse>
{
    private readonly IOracleHcmDiagnosticsService _service;
    private readonly ILogger<GetOracleHcmSyncMetadataEndpoint> _logger;

    public GetOracleHcmSyncMetadataEndpoint(IOracleHcmDiagnosticsService service, ILogger<GetOracleHcmSyncMetadataEndpoint> logger)
        : base(Navigation.Constants.OracleHcmDiagnostics)
    {
        _service = service;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("oracleHcm/metadata");
        Summary(s =>
        {
            s.Summary = "Gets OracleHcm sync metadata with timestamp information";
            s.Description = "Returns the most recent create and modify timestamps from Demographic and PayProfit tables";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new OracleHcmSyncMetadataResponse
                    {
                        DemographicCreatedAtUtc = DateTimeOffset.UtcNow,
                        DemographicModifiedAtUtc = DateTimeOffset.UtcNow,
                        PayProfitCreatedAtUtc = DateTimeOffset.UtcNow,
                        PayProfitModifiedAtUtc = DateTimeOffset.UtcNow
                    }
                }
            };
        });
        Group<ItDevOpsGroup>();
    }

    public override Task<OracleHcmSyncMetadataResponse> ExecuteAsync(CancellationToken ct)
    {
        return ExecuteAsyncInternal(ct);
    }

    private async Task<OracleHcmSyncMetadataResponse> ExecuteAsyncInternal(CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            var result = await _service.GetOracleHcmSyncMetadataAsync(ct);

            if (result.IsError)
            {
                _logger.LogError("Failed to get OracleHcm sync metadata: {Error}", result.Error?.Description);
                return new OracleHcmSyncMetadataResponse();
            }

            var response = new OracleHcmSyncMetadataResponse
            {
                DemographicCreatedAtUtc = result.Value!.DemographicCreatedAtUtc,
                DemographicModifiedAtUtc = result.Value!.DemographicModifiedAtUtc,
                PayProfitCreatedAtUtc = result.Value!.PayProfitCreatedAtUtc,
                PayProfitModifiedAtUtc = result.Value!.PayProfitModifiedAtUtc
            };

            // Business metrics
            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "oracleHcm-metadata-query"),
                    new("endpoint", "GetOracleHcmSyncMetadataEndpoint"));
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
