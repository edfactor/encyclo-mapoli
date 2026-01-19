using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

/// <summary>
/// Gets demographic sync audit records with pagination support, ordered by creation date (descending).
/// </summary>
public class GetDemographicSyncAuditEndpoint
    : ProfitSharingEndpoint<SortedPaginationRequestDto, Results<Ok<PaginatedResponseDto<DemographicSyncAuditRecordResponse>>, NotFound, ProblemHttpResult>>
{
    private readonly IOracleHcmDiagnosticsService _service;

    public GetDemographicSyncAuditEndpoint(IOracleHcmDiagnosticsService service)
        : base(Navigation.Constants.OracleHcmDiagnostics)
    {
        _service = service;
    }

    public override void Configure()
    {
        Get("oracle-hcm/audit");
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
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<PaginatedResponseDto<DemographicSyncAuditRecordResponse>>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        SortedPaginationRequestDto req,
        CancellationToken ct)
    {
        var result = await _service.GetDemographicSyncAuditAsync(req, ct);
        return result.ToHttpResult();
    }
}
