using Demoulas.ProfitSharing.Common.Contracts.Response.Audit;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Audit;

public sealed class GetAuditChangeEntryEndpoint : ProfitSharingEndpoint<GetAuditChangeEntryRequest, List<AuditChangeEntryDto>>
{
    private readonly IAuditService _auditService;
    private readonly ILogger<GetAuditChangeEntryEndpoint> _logger;

    public GetAuditChangeEntryEndpoint(IAuditService auditService, ILogger<GetAuditChangeEntryEndpoint> logger)
        : base(Navigation.Constants.ItDevOps)
    {
        _auditService = auditService;
        _logger = logger;
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

    public override Task<List<AuditChangeEntryDto>> ExecuteAsync(GetAuditChangeEntryRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var response = await _auditService.GetAuditChangeEntriesAsync(req.Id, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "audit-change-entry-lookup"),
                new("endpoint", "GetAuditChangeEntryEndpoint"));

            var changeCount = response?.Count ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(changeCount,
                new("record_type", "audit-change-entries"),
                new("endpoint", "GetAuditChangeEntryEndpoint"));

            _logger.LogInformation("Audit change entries retrieved for EventId: {EventId}, returned {ChangeCount} changes (correlation: {CorrelationId})",
                req.Id, changeCount, HttpContext.TraceIdentifier);

            return response ?? new List<AuditChangeEntryDto>();
        });
    }
}

public sealed record GetAuditChangeEntryRequest
{
    public int Id { get; init; }
}
