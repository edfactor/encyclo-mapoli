using Demoulas.ProfitSharing.Common.Contracts;

namespace Demoulas.ProfitSharing.Common.Interfaces.CheckRun;

/// <summary>
/// Orchestrates the complete check run process: file generation (MICR, DJDE, PositivePay) and FTP transfer.
/// </summary>
public interface ICheckRunOrchestrator
{
    /// <summary>
    /// Executes the complete check run workflow for a profit year.
    /// Generates MICR, DJDE, and PositivePay files, then transfers them via FTP.
    /// Logs all operations to FtpOperationLog for audit trail.
    /// </summary>
    /// <param name="profitYear">The profit year for the check run.</param>
    /// <param name="checkNumber">The check number identifier.</param>
    /// <param name="userName">The username initiating the check run.</param>
    /// <param name="runId">The workflow run ID for correlation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure with error details.</returns>
    Task<Result<bool>> ExecuteCheckRunAsync(short profitYear,
        string checkNumber,
        string userName,
        Guid runId,
        CancellationToken cancellationToken);
}
