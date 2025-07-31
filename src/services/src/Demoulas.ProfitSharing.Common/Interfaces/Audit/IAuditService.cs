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
        Func<Task<TResponse>> reportFunction,
        CancellationToken cancellationToken)
        where TResponse : ReportResponseBase<TResult>
        where TRequest : PaginationRequestDto
        where TResult : class;
}
