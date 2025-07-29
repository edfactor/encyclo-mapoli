using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces.Audit;
public interface IAuditService
{
    Task ArchiveCompletedReportAsync<TRequest, TReport>(string reportName,
        short profitYear,
        TRequest request,
        TReport report,
        List<KeyValuePair<string, decimal>> kevValues,
        CancellationToken cancellationToken)
        where TReport : class where TRequest : class;
}
