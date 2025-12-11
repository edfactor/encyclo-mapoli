using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.Audit;
using Demoulas.ProfitSharing.Common.Contracts.Response.Audit;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Audit;

public class AuditSearchEndpoint : ProfitSharingEndpoint<AuditSearchRequestDto, PaginatedResponseDto<AuditEventDto>>
{
    private readonly IAuditService _auditService;
    private readonly ILogger<AuditSearchEndpoint> _logger;

    public AuditSearchEndpoint(IAuditService auditService, ILogger<AuditSearchEndpoint> logger)
        : base(Navigation.Constants.ItDevOps)
    {
        _auditService = auditService;
        _logger = logger;
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

    public override Task<PaginatedResponseDto<AuditEventDto>> ExecuteAsync(AuditSearchRequestDto req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var response = await _auditService.SearchAuditEventsAsync(req, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "audit-search"),
                new("endpoint", "AuditSearchEndpoint"));

            var resultCount = response?.Total ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "audit-events"),
                new("endpoint", "AuditSearchEndpoint"));

            _logger.LogInformation("Audit search completed for Table: {TableName}, Operation: {Operation}, User: {UserName}, returned {ResultCount} results (correlation: {CorrelationId})",
                req.TableName, req.Operation, req.UserName, resultCount, HttpContext.TraceIdentifier);

            return response ?? new PaginatedResponseDto<AuditEventDto>();
        });
    }
}
