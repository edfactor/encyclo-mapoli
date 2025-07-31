using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces.Audit;
public interface IAuditService
{
    Task<TResponse> ArchiveCompletedReportAsync<TRequest, TResponse, TResult>(
        string reportName,
        short profitYear,
        TRequest request,
        Func<TRequest, CancellationToken, Task<TResponse>> reportFunction,
        CancellationToken cancellationToken)
        where TResponse : ReportResponseBase<TResult>
        where TRequest : PaginationRequestDto
        where TResult : class;

    Task ArchiveCompletedReportAsync<TRequest, TResponse>(
        string reportName,
        TRequest request,
        TResponse response,
        CancellationToken cancellationToken)
        where TResponse : class
        where TRequest : PaginationRequestDto;
}
