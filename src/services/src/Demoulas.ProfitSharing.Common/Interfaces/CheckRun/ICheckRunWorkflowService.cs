using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.CheckRun;

namespace Demoulas.ProfitSharing.Common.Interfaces.CheckRun;

/// <summary>
/// Service for managing check run workflow operations including reprint tracking.
/// </summary>
public interface ICheckRunWorkflowService
{
    /// <summary>
    /// Gets the current check run workflow for the specified profit year.
    /// </summary>
    /// <param name="profitYear">The profit year to retrieve the check run for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current check run workflow or failure if not found.</returns>
    Task<Result<CheckRunWorkflowResponse>> GetCurrentRunAsync(short profitYear, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts a new check run workflow for the specified profit year.
    /// </summary>
    /// <param name="profitYear">The profit year for the check run.</param>
    /// <param name="checkRunDate">The date of the check run.</param>
    /// <param name="checkNumber">The starting check number for the run.</param>
    /// <param name="userName">The username initiating the check run.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created check run workflow.</returns>
    Task<Result<CheckRunWorkflowResponse>> StartNewRunAsync(short profitYear,
        DateOnly checkRunDate,
        int checkNumber,
        string userName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Records the completion of a workflow step.
    /// </summary>
    /// <param name="runId">The check run workflow ID.</param>
    /// <param name="stepNumber">The step number being completed.</param>
    /// <param name="userName">The username completing the step.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success if the step was recorded, failure if the workflow was not found.</returns>
    Task<Result<bool>> RecordStepCompletionAsync(
        Guid runId,
        int stepNumber,
        string userName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines if a check run can be reprinted based on reprint count and date constraints.
    /// </summary>
    /// <param name="runId">The check run workflow ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the check run can be reprinted, false otherwise.</returns>
    Task<Result<bool>> CanReprintAsync(Guid runId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Increments the reprint count for a check run workflow.
    /// </summary>
    /// <param name="runId">The check run workflow ID.</param>
    /// <param name="userName">The username initiating the reprint.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success if the reprint count was incremented, failure if the workflow was not found.</returns>
    Task<Result<bool>> IncrementReprintCountAsync(
        Guid runId,
        string userName,
        CancellationToken cancellationToken = default);
}
