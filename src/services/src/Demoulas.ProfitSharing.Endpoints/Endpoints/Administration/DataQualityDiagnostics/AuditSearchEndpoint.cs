using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.Audit;
using Demoulas.ProfitSharing.Common.Contracts.Response.Audit;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Audit;

public class AuditSearchEndpoint : ProfitSharingEndpoint<AuditSearchRequestDto, PaginatedResponseDto<AuditEventDto>>
{
    private readonly IAuditService _auditService;

    public AuditSearchEndpoint(IAuditService auditService)
        : base(Navigation.Constants.ItDevOps)
    {
        _auditService = auditService;
    }

    public override void Configure()
    {
        Get("search");
        Summary(m =>
        {
            m.Summary = "Search audit events with filtering and pagination";
            m.Description = "Returns audit events filtered by table name, operation, username, and date range. ChangesJson is only included when TableName is 'NAVIGATION'.";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new PaginatedResponseDto<AuditEventDto>() } };
        });
        Group<AuditGroup>();
    }

    protected override async Task<PaginatedResponseDto<AuditEventDto>> HandleRequestAsync(
        AuditSearchRequestDto req,
        CancellationToken ct)
    {
        var result = await _auditService.SearchAuditEventsAsync(req, ct);
        return result ?? new PaginatedResponseDto<AuditEventDto>();
    }
}
