using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.CheckRun;
using Demoulas.ProfitSharing.Common.Contracts.Response.CheckRun;

namespace Demoulas.ProfitSharing.Common.Interfaces.CheckRun;

/// <summary>
/// Generates printer-ready output for an explicit set of distributions and records check tracking.
/// </summary>
public interface ICheckRunPrintFileService
{
    Task<Result<CheckRunPrintFileResult>> GenerateAsync(CheckRunStartRequest request, CancellationToken cancellationToken);
}
