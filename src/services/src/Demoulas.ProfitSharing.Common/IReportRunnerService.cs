namespace Demoulas.ProfitSharing.Services.Internal.Interfaces;

public interface IReportRunnerService
{
    Task<Dictionary<string, object>> IncludeReportInformation(string reportSelector, CancellationToken cancellationToken);
}
