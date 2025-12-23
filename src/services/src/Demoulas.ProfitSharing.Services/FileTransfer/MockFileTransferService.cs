using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.FileTransfer;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.FileTransfer;

/// <summary>
/// Mock implementation of file transfer service for development and testing.
/// Uses .NET built-in retry logic for resilience.
/// Production implementation will use SFTP client to transfer to /production/OutBox/VENUS/.
/// Creates FtpOperationLog entries for audit trail.
/// </summary>
public sealed class MockFileTransferService : IFileTransferService
{
    private readonly ILogger<MockFileTransferService> _logger;
    private readonly IProfitSharingDataContextFactory _contextFactory;

    public MockFileTransferService(
        ILogger<MockFileTransferService> logger,
        IProfitSharingDataContextFactory contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    /// <summary>
    /// Transfers file content to destination via SFTP (mocked).
    /// Creates FtpOperationLog entry for audit trail.
    /// </summary>
    public async Task<Result<FileTransferAudit>> TransferFileAsync(
        Stream content,
        string destination,
        string fileName,
        CancellationToken cancellationToken)
    {
        var startTime = DateTimeOffset.UtcNow;
        Guid? checkRunWorkflowId = null; // Will be set when check run context is available

        var audit = new FileTransferAudit
        {
            Id = Guid.NewGuid(),
            Timestamp = startTime,
            FileName = fileName,
            Destination = destination,
            UserId = Guid.Empty, // Will be set from HTTP context in production
            IsSuccess = false
        };

        try
        {
            // Read content into byte array for audit storage and file size calculation
            await using var memoryStream = new MemoryStream();
            await content.CopyToAsync(memoryStream, cancellationToken);
            var csvContent = memoryStream.ToArray();
            audit.CsvContent = csvContent;
            audit.FileSize = csvContent.Length;

            // Execute transfer with retry logic (3 attempts)
            var maxAttempts = 3;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    _logger.LogInformation(
                        "MOCK: Transferring file {FileName} ({FileSize} bytes) to {Destination} (attempt {Attempt}/{MaxAttempts})",
                        fileName, csvContent.Length, destination, attempt, maxAttempts);

                    // Simulate network latency
                    await Task.Delay(100, cancellationToken);

                    _logger.LogInformation(
                        "MOCK: Successfully transferred file {FileName} to {Destination}",
                        fileName, destination);

                    break; // Success, exit retry loop
                }
                catch (Exception ex) when (attempt < maxAttempts && (ex is IOException || ex is HttpRequestException))
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    _logger.LogWarning(ex,
                        "File transfer attempt {Attempt} failed. Retrying in {RetryDelay}s...",
                        attempt, delay.TotalSeconds);

                    await Task.Delay(delay, cancellationToken);
                }
            }

            audit.IsSuccess = true;
            audit.TransferDurationMs = (long)(DateTimeOffset.UtcNow - startTime).TotalMilliseconds;

            _logger.LogInformation(
                "File transfer completed successfully in {Duration}ms. File: {FileName}, Size: {FileSize} bytes",
                audit.TransferDurationMs, fileName, audit.FileSize);

            // Create FtpOperationLog entry for successful transfer
            await CreateFtpOperationLogAsync(
                checkRunWorkflowId,
                FtpOperationType.Upload,
                fileName,
                destination,
                isSuccess: true,
                errorMessage: null,
                durationMs: audit.TransferDurationMs,
                userId: audit.UserId,
                cancellationToken);

            return Result<FileTransferAudit>.Success(audit);
        }
        catch (Exception ex)
        {
            audit.IsSuccess = false;
            audit.ErrorMessage = ex.Message;
            audit.TransferDurationMs = (long)(DateTimeOffset.UtcNow - startTime).TotalMilliseconds;

            _logger.LogError(ex,
                "File transfer failed after retries. File: {FileName}, Destination: {Destination}, Error: {Error}",
                fileName, destination, ex.Message);

            // Create FtpOperationLog entry for failed transfer
            await CreateFtpOperationLogAsync(
                checkRunWorkflowId,
                FtpOperationType.Upload,
                fileName,
                destination,
                isSuccess: false,
                errorMessage: ex.Message,
                durationMs: audit.TransferDurationMs,
                userId: audit.UserId,
                cancellationToken);

            return Result<FileTransferAudit>.Failure(
                Error.FileTransferFailed with { Description = $"Failed to transfer file {fileName}: {ex.Message}" });
        }
    }

    /// <summary>
    /// Creates an FtpOperationLog entry in the database for audit trail.
    /// </summary>
    private async Task CreateFtpOperationLogAsync(
        Guid? checkRunWorkflowId,
        FtpOperationType operationType,
        string fileName,
        string destination,
        bool isSuccess,
        string? errorMessage,
        long durationMs,
        Guid userId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _contextFactory.UseWritableContext(async context =>
            {
                var operationLog = new FtpOperationLog
                {
                    CheckRunWorkflowId = checkRunWorkflowId ?? Guid.Empty,
                    OperationType = operationType,
                    FileName = fileName,
                    Destination = destination,
                    IsSuccess = isSuccess,
                    ErrorMessage = errorMessage,
                    DurationMs = durationMs,
                    Timestamp = DateTimeOffset.UtcNow,
                    UserId = userId
                };

                context.FtpOperationLogs.Add(operationLog);
                await context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "FtpOperationLog created: Operation={Operation}, File={FileName}, Success={IsSuccess}, Duration={Duration}ms",
                    operationType, fileName, isSuccess, durationMs);

                return Result<bool>.Success(true);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the entire transfer if audit logging fails
            _logger.LogError(ex,
                "Failed to create FtpOperationLog entry for file {FileName}. Transfer completed but audit logging failed.",
                fileName);
        }
    }
}
