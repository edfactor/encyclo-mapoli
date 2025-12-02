using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.Audit;
using Demoulas.ProfitSharing.Common.Contracts.Response.Audit;

namespace Demoulas.ProfitSharing.Common.Interfaces.Audit;

public interface IAuditService
{
    Task<TResponse> ArchiveCompletedReportAsync<TRequest, TResponse>(
        string reportName,
        short profitYear,
        TRequest request,
        Func<TRequest, bool, CancellationToken, Task<TResponse>> reportFunction,
        CancellationToken cancellationToken)
        where TResponse : class
        where TRequest : PaginationRequestDto;

    Task<TResponse> ArchiveCompletedReportAsync<TRequest, TResponse>(
        string reportName,
        short profitYear,
        TRequest request,
        bool isArchiveRequest,
        Func<TRequest, bool, CancellationToken, Task<TResponse>> reportFunction,
        CancellationToken cancellationToken)
        where TResponse : class
        where TRequest : PaginationRequestDto;

    /// <summary>
    /// Searches audit events with filtering and pagination.
    /// </summary>
    /// <param name="request">The search request with filter criteria and pagination settings.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated response containing matching audit events.</returns>
    Task<PaginatedResponseDto<AuditEventDto>> SearchAuditEventsAsync(
        AuditSearchRequestDto request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the change entries for a specific audit event.
    /// </summary>
    /// <param name="auditEventId">The ID of the audit event.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of audit change entry DTOs.</returns>
    Task<List<AuditChangeEntryDto>> GetAuditChangeEntriesAsync(
        int auditEventId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Logs sensitive data access for compliance and security auditing.
    /// </summary>
    /// <param name="operationName">The name of the operation being logged (e.g., "Get Unmasked SSN").</param>
    /// <param name="tableName">The name of the table being accessed.</param>
    /// <param name="primaryKey">The primary key or identifier of the record accessed.</param>
    /// <param name="details">Additional details about the access.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task LogSensitiveDataAccessAsync(
        string operationName,
        string tableName,
        string? primaryKey,
        string? details,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs sensitive data access and executes an operation atomically.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the operation.</typeparam>
    /// <param name="operationName">The name of the operation being logged.</param>
    /// <param name="tableName">The name of the table being accessed.</param>
    /// <param name="primaryKey">The primary key or identifier of the record accessed.</param>
    /// <param name="details">Additional details about the access.</param>
    /// <param name="operation">The async operation to execute after logging.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<TResult> LogSensitiveDataAccessAsync<TResult>(
        string operationName,
        string tableName,
        string? primaryKey,
        string? details,
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
        where TResult : notnull;
}
