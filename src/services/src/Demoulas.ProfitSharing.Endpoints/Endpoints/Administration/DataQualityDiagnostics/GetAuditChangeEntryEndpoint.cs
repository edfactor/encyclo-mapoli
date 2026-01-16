using Demoulas.ProfitSharing.Common.Contracts.Response.Audit;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Audit;

public sealed class GetAuditChangeEntryEndpoint : ProfitSharingEndpoint<GetAuditChangeEntryRequest, List<AuditChangeEntryDto>>
{
    private readonly IAuditService _auditService;

    public GetAuditChangeEntryEndpoint(IAuditService auditService)
        : base(Navigation.Constants.ItDevOps)
    {
        _auditService = auditService;
    }

    public override void Configure()
    {
        Get("changes/{id}");
        Summary(m =>
        {
            m.Summary = "Get audit change entries for a specific audit event";
            m.Description = "Returns the list of change entries (original and new values) for a specific audit event by ID.";
            m.ExampleRequest = new GetAuditChangeEntryRequest { Id = 123 };
            m.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200, new List<AuditChangeEntryDto>
                    {
                        new()
                        {
                            Id = 1,
                            ColumnName = "StatusId",
                            OriginalValue = "1",
                            NewValue = "2"
                        }
                    }
                }
            };
        });
        Group<AuditGroup>();
    }

    protected override async Task<List<AuditChangeEntryDto>> HandleRequestAsync(GetAuditChangeEntryRequest req, CancellationToken ct)
    {
        var result = await _auditService.GetAuditChangeEntriesAsync(req.Id, ct);
        return result ?? new List<AuditChangeEntryDto>();
    }
}

public sealed record GetAuditChangeEntryRequest
{
    public int Id { get; init; }
}
