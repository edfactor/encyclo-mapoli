using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces.Audit;
public interface IAuditService
{
   Task ArchiveCompletedReportAsync<TRequest, TReport>(string reportName,
        TRequest request,
        TReport report,
        CancellationToken cancellationToken)
        where TReport : class where TRequest : IProfitYearRequest;
}
