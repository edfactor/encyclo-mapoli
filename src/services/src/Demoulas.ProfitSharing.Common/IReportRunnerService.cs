namespace Demoulas.ProfitSharing.Common;

public interface IReportRunnerService
{
    Task<Dictionary<string, object>> IncludeReportInformation(string reportSelector, CancellationToken cancellationToken);
}
