using Demoulas.Common.Contracts.Contracts.Request.Audit;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Contracts.Contracts.Response.Audit;
using Demoulas.ProfitSharing.Common.Contracts.Request.Audit;
using Demoulas.ProfitSharing.Common.Contracts.Response.Audit;

namespace Demoulas.ProfitSharing.Services.Audit;

public sealed class CommonAuditServiceAdapter : Demoulas.Common.Contracts.Interfaces.Audit.IAuditService
{
    private readonly Demoulas.ProfitSharing.Common.Interfaces.Audit.IProfitSharingAuditService _profitSharingAuditService;

    public CommonAuditServiceAdapter(Demoulas.ProfitSharing.Common.Interfaces.Audit.IProfitSharingAuditService profitSharingAuditService)
    {
        _profitSharingAuditService = profitSharingAuditService;
    }

    public async Task<PaginatedResponseDto<AuditEventResponse>> SearchAuditEventsAsync(
        AuditSearchRequestRequest request,
        CancellationToken cancellationToken)
    {
        var mappedRequest = new AuditSearchRequestDto
        {
            TableName = request.TableName,
            Operation = request.Operation,
            UserName = request.UserName,
            SessionId = request.SessionId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SortBy = request.SortBy,
            IsSortDescending = request.IsSortDescending,
            Skip = request.Skip,
            Take = request.Take
        };

        var result = await _profitSharingAuditService.SearchAuditEventsAsync(mappedRequest, cancellationToken);
        return new PaginatedResponseDto<AuditEventResponse>
        {
            Results = result.Results?.Select(MapAuditEvent).ToList() ?? [],
            Total = result.Total
        };
    }

    public async Task<List<AuditChangeEntryResponse>> GetAuditChangeEntriesAsync(
        int auditEventId,
        CancellationToken cancellationToken)
    {
        var result = await _profitSharingAuditService.GetAuditChangeEntriesAsync(auditEventId, cancellationToken);
        return result.Select(MapAuditChangeEntry).ToList();
    }

    public Task LogSensitiveDataAccessAsync(
        string operationName,
        string tableName,
        string? primaryKey,
        string? details,
        CancellationToken cancellationToken = default)
    {
        return _profitSharingAuditService.LogSensitiveDataAccessAsync(operationName, tableName, primaryKey, details, cancellationToken);
    }

    public Task<TResult> LogSensitiveDataAccessAsync<TResult>(
        string operationName,
        string tableName,
        string? primaryKey,
        string? details,
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        return _profitSharingAuditService.LogSensitiveDataAccessAsync(operationName, tableName, primaryKey, details, operation, cancellationToken);
    }

    public Task LogDataChangeAsync(
        string operationName,
        string tableName,
        string auditOperation,
        string? primaryKey,
        IReadOnlyList<AuditChangeEntryInputRequest> changes,
        CancellationToken cancellationToken = default)
    {
        var mappedChanges = changes.Select(change => new AuditChangeEntryInput
        {
            ColumnName = change.ColumnName,
            OriginalValue = change.OriginalValue,
            NewValue = change.NewValue
        }).ToList();

        return _profitSharingAuditService.LogDataChangeAsync(operationName, tableName, auditOperation, primaryKey, mappedChanges, cancellationToken);
    }

    private static AuditEventResponse MapAuditEvent(AuditEventDto auditEvent)
    {
        return new AuditEventResponse
        {
            AuditEventId = auditEvent.AuditEventId,
            TableName = auditEvent.TableName,
            Operation = auditEvent.Operation,
            PrimaryKey = auditEvent.PrimaryKey,
            UserName = auditEvent.UserName,
            CreatedAt = auditEvent.CreatedAt,
            ChangesJson = auditEvent.ChangesJson?.Select(MapAuditChangeEntry).ToList()
        };
    }

    private static AuditChangeEntryResponse MapAuditChangeEntry(AuditChangeEntryDto entry)
    {
        return new AuditChangeEntryResponse
        {
            Id = entry.Id,
            ColumnName = entry.ColumnName,
            OriginalValue = entry.OriginalValue,
            NewValue = entry.NewValue
        };
    }
}
