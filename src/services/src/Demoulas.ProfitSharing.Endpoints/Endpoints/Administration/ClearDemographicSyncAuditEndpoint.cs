using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

/// <summary>
/// Clears all records from the DEMOGRAPHIC_SYNC_AUDIT table.
/// Returns the count of deleted records.
/// </summary>
public class ClearDemographicSyncAuditEndpoint
    : ProfitSharingEndpoint<EmptyRequest, Results<Ok<ClearAuditResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IOracleHcmDiagnosticsService _service;

    public ClearDemographicSyncAuditEndpoint(IOracleHcmDiagnosticsService service)
        : base(Navigation.Constants.OracleHcmDiagnostics)
    {
        _service = service;
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

    protected override async Task<Results<Ok<ClearAuditResponse>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        EmptyRequest req,
        CancellationToken ct)
    {
        var result = await _service.ClearDemographicSyncAuditAsync(ct);
        var responseResult = result.Match(
            v => Result<ClearAuditResponse>.Success(new ClearAuditResponse { DeletedCount = v }),
            _ => Result<ClearAuditResponse>.Failure(result.Error!));

        return responseResult.ToHttpResult();
    }
}
