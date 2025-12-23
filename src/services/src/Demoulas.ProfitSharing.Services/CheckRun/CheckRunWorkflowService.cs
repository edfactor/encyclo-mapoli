using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.CheckRun;
using Demoulas.ProfitSharing.Common.Interfaces.CheckRun;
using Demoulas.ProfitSharing.Data.Entities.CheckRun;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.CheckRun;

/// <summary>
/// Service for managing check run workflows including state tracking and reprint limit enforcement.
/// </summary>
public sealed class CheckRunWorkflowService : ICheckRunWorkflowService
{
    private readonly IProfitSharingDataContextFactory _factory;
    private readonly ILogger<CheckRunWorkflowService> _logger;

    public CheckRunWorkflowService(
        IProfitSharingDataContextFactory factory,
        ILogger<CheckRunWorkflowService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<CheckRunWorkflowResponse>> GetCurrentRunAsync(int profitYear, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving current check run workflow for profit year {ProfitYear}", profitYear);

            return await _factory.UseReadOnlyContext(async context =>
            {
                var workflow = await context.CheckRunWorkflows
                    .FirstOrDefaultAsync(w => w.ProfitYear == profitYear && w.StepStatus != CheckRunStepStatus.Failed, cancellationToken);

                if (workflow is null)
                {
                    _logger.LogWarning("No active check run workflow found for profit year {ProfitYear}", profitYear);
                    return Result<CheckRunWorkflowResponse>.Failure(Error.EntityNotFound(
                        $"No active check run found for profit year {profitYear}"));
                }

                _logger.LogInformation("Found active check run workflow {RunId} for profit year {ProfitYear} at step {StepNumber} with status {StepStatus}",
                    workflow.Id, profitYear, workflow.StepNumber, workflow.StepStatus);

                return Result<CheckRunWorkflowResponse>.Success(MapToDto(workflow));
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving check run workflow for profit year {ProfitYear}: {ErrorMessage}",
                profitYear, ex.Message);
            return Result<CheckRunWorkflowResponse>.Failure(Error.Unexpected(ex.Message));
        }
    }

    /// <inheritdoc/>
    public async Task<Result<CheckRunWorkflowResponse>> StartNewRunAsync(
        int profitYear,
        DateOnly checkRunDate,
        int checkNumber,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting new check run workflow for profit year {ProfitYear} with check number {CheckNumber} on {CheckRunDate} by user {UserId}",
                profitYear, checkNumber, checkRunDate, userId);

            return await _factory.UseWritableContext(async context =>
            {
                // Validate no active run exists for this year
                var existingRun = await context.CheckRunWorkflows
                    .FirstOrDefaultAsync(w => w.ProfitYear == profitYear && w.StepStatus != CheckRunStepStatus.Failed, cancellationToken);

                if (existingRun is not null)
                {
                    _logger.LogWarning("Active check run workflow {RunId} already exists for profit year {ProfitYear}",
                        existingRun.Id, profitYear);
                    return Result<CheckRunWorkflowResponse>.Failure(Error.Unexpected(
                        $"An active check run already exists for profit year {profitYear}"));
                }

                // Create new workflow
                var workflow = new CheckRunWorkflow
                {
                    Id = Guid.NewGuid(),
                    ProfitYear = profitYear,
                    CheckRunDate = checkRunDate,
                    StepNumber = 1,
                    StepStatus = CheckRunStepStatus.Pending,
                    CheckNumber = checkNumber,
                    ReprintCount = 0,
                    MaxReprintCount = 2,
                    CreatedByUserId = userId,
                    CreatedDate = DateTimeOffset.UtcNow
                };

                context.CheckRunWorkflows.Add(workflow);
                await context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created new check run workflow {RunId} for profit year {ProfitYear} with check number {CheckNumber}",
                    workflow.Id, profitYear, checkNumber);

                return Result<CheckRunWorkflowResponse>.Success(MapToDto(workflow));
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting new check run workflow for profit year {ProfitYear}: {ErrorMessage}",
                profitYear, ex.Message);
            return Result<CheckRunWorkflowResponse>.Failure(Error.Unexpected(ex.Message));
        }
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> RecordStepCompletionAsync(
        Guid runId,
        int stepNumber,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Recording step {StepNumber} completion for check run workflow {RunId} by user {UserId}",
                stepNumber, runId, userId);

            return await _factory.UseWritableContext(async context =>
            {
                var workflow = await context.CheckRunWorkflows.FirstOrDefaultAsync(w => w.Id == runId, cancellationToken);

                if (workflow is null)
                {
                    _logger.LogWarning("Check run workflow {RunId} not found", runId);
                    return Result<bool>.Failure(Error.EntityNotFound(
                        $"Check run workflow {runId} not found"));
                }

                // Update workflow step
                workflow.StepStatus = CheckRunStepStatus.Completed;
                workflow.StepNumber++;
                workflow.ModifiedByUserId = userId;
                workflow.ModifiedDate = DateTimeOffset.UtcNow;

                await context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Completed step {StepNumber} for check run workflow {RunId}, advanced to step {NextStepNumber}",
                    stepNumber, runId, workflow.StepNumber);

                return Result<bool>.Success(true);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording step completion for check run workflow {RunId}: {ErrorMessage}",
                runId, ex.Message);
            return Result<bool>.Failure(Error.Unexpected(ex.Message));
        }
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> CanReprintAsync(Guid runId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Checking reprint eligibility for check run workflow {RunId}", runId);

            return await _factory.UseReadOnlyContext(async context =>
            {
                var workflow = await context.CheckRunWorkflows.FirstOrDefaultAsync(w => w.Id == runId, cancellationToken);

                if (workflow is null)
                {
                    _logger.LogWarning("Check run workflow {RunId} not found", runId);
                    return Result<bool>.Failure(
                        Error.EntityNotFound($"Check run workflow {runId} not found"));
                }

                // Critical business logic: 2 max reprints, same day only
                bool canReprint = workflow.ReprintCount < workflow.MaxReprintCount &&
                                 workflow.CheckRunDate == DateOnly.FromDateTime(DateTime.Today);

                _logger.LogInformation(
                    "Reprint eligibility check for run {RunId}: ReprintCount={ReprintCount}, MaxReprintCount={MaxReprintCount}, " +
                    "CheckRunDate={CheckRunDate}, Today={Today}, CanReprint={CanReprint}",
                    runId, workflow.ReprintCount, workflow.MaxReprintCount, workflow.CheckRunDate,
                    DateOnly.FromDateTime(DateTime.Today), canReprint);

                return Result<bool>.Success(canReprint);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking reprint eligibility for check run workflow {RunId}: {ErrorMessage}",
                runId, ex.Message);
            return Result<bool>.Failure(Error.Unexpected(ex.Message));
        }
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> IncrementReprintCountAsync(
        Guid runId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Incrementing reprint count for check run workflow {RunId} by user {UserId}",
                runId, userId);

            return await _factory.UseWritableContext(async context =>
            {
                var workflow = await context.CheckRunWorkflows.FirstOrDefaultAsync(w => w.Id == runId, cancellationToken);

                if (workflow is null)
                {
                    _logger.LogWarning("Check run workflow {RunId} not found", runId);
                    return Result<bool>.Failure(Error.EntityNotFound(
                        $"Check run workflow {runId} not found"));
                }

                // Increment reprint count
                workflow.ReprintCount++;
                workflow.ModifiedByUserId = userId;
                workflow.ModifiedDate = DateTimeOffset.UtcNow;

                await context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Incremented reprint count to {ReprintCount} for check run workflow {RunId}",
                    workflow.ReprintCount, runId);

                return Result<bool>.Success(true);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing reprint count for check run workflow {RunId}: {ErrorMessage}",
                runId, ex.Message);
            return Result<bool>.Failure(Error.Unexpected(ex.Message));
        }
    }

    /// <summary>
    /// Maps a CheckRunWorkflow entity to a CheckRunWorkflowResponse DTO.
    /// </summary>
    private static CheckRunWorkflowResponse MapToDto(CheckRunWorkflow workflow)
    {
        return new CheckRunWorkflowResponse
        {
            Id = workflow.Id,
            ProfitYear = workflow.ProfitYear,
            CheckRunDate = workflow.CheckRunDate,
            StepNumber = workflow.StepNumber,
            StepStatus = workflow.StepStatus,
            CheckNumber = workflow.CheckNumber,
            ReprintCount = workflow.ReprintCount,
            MaxReprintCount = workflow.MaxReprintCount,
            CreatedByUserId = workflow.CreatedByUserId,
            CreatedDate = workflow.CreatedDate,
            ModifiedByUserId = workflow.ModifiedByUserId,
            ModifiedDate = workflow.ModifiedDate
        };
    }
}
