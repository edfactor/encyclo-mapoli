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
}
