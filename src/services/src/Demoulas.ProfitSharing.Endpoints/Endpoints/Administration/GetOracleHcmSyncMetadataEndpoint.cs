using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts;
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
/// Gets OracleHcm sync metadata including the most recent create and modify timestamps
/// from Demographic and PayProfit tables.
/// </summary>
public class GetOracleHcmSyncMetadataEndpoint
    : ProfitSharingEndpoint<EmptyRequest, Results<Ok<OracleHcmSyncMetadataResponse>, NotFound, ProblemHttpResult>>
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
        Group<AdministrationGroup>();
    }

    public override Task<Results<Ok<OracleHcmSyncMetadataResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(
        EmptyRequest req,
        CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _service.GetOracleHcmSyncMetadataAsync(ct);

            var responseResult = result.Match(
                v => Result<OracleHcmSyncMetadataResponse>.Success(new OracleHcmSyncMetadataResponse
                {
                    DemographicCreatedAtUtc = v.DemographicCreatedAtUtc,
                    DemographicModifiedAtUtc = v.DemographicModifiedAtUtc,
                    PayProfitCreatedAtUtc = v.PayProfitCreatedAtUtc,
                    PayProfitModifiedAtUtc = v.PayProfitModifiedAtUtc
                }),
                _ => Result<OracleHcmSyncMetadataResponse>.Failure(result.Error!));

            if (responseResult.IsSuccess)
            {
                EndpointTelemetry.BusinessOperationsTotal.Add(1,
                    new("operation", "oracleHcm-metadata-query"),
                    new("endpoint", nameof(GetOracleHcmSyncMetadataEndpoint)));
            }

            return responseResult.ToHttpResult();
        });
    }
}
