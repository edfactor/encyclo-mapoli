using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces.Audit;
public interface IAuditService
{
    Task ArchiveCompletedReportAsync<TReport>(string reportName, TReport report, CancellationToken cancellationToken)
        where TReport : class;
}
