using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

/// <summary>
/// Gets OracleHcm sync metadata including the most recent create and modify timestamps
/// from Demographic and PayProfit tables.
/// </summary>
public class GetOracleHcmSyncMetadataEndpoint
    : ProfitSharingEndpoint<EmptyRequest, Results<Ok<OracleHcmSyncMetadataResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IOracleHcmDiagnosticsService _service;

    public GetOracleHcmSyncMetadataEndpoint(IOracleHcmDiagnosticsService service)
        : base(Navigation.Constants.OracleHcmDiagnostics)
    {
        _service = service;
    }

    public override void Configure()
    {
        Get("oracle-hcm/metadata");
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

    protected override async Task<Results<Ok<OracleHcmSyncMetadataResponse>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        EmptyRequest req,
        CancellationToken ct)
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

        return responseResult.ToHttpResult();
    }
}
