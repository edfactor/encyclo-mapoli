using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Interfaces.Audit;

namespace Demoulas.ProfitSharing.Common.Interfaces.Audit;

public interface IProfitSharingAuditService : IAuditService
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
        Func<TRequest, bool, CancellationToken, Task<TResponse>> reportFunction,
        List<Func<TResponse, (string, object)>> additionalChecksums,
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

    Task<TResponse> ArchiveCompletedReportAsync<TRequest, TResponse>(
        string reportName,
        short profitYear,
        TRequest request,
        bool isArchiveRequest,
        Func<TRequest, bool, CancellationToken, Task<TResponse>> reportFunction,
        List<Func<TResponse, (string, object)>> additionalChecksums,
        CancellationToken cancellationToken)
        where TResponse : class
        where TRequest : PaginationRequestDto;
}
