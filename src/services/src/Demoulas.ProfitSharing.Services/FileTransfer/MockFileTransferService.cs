using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Interfaces.FileTransfer;
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
    /// Creates FtpOperationLog and FileTransferAudit entries for audit trail.
    /// </summary>
    public async Task<Result<bool>> TransferFileAsync(
        Stream content,
        string destination,
        string fileName,
        CancellationToken cancellationToken)
    {
        var startTime = DateTimeOffset.UtcNow;
        Guid? checkRunWorkflowId = null; // Will be set when check run context is available

        long fileSize = 0;
        byte[] csvContent = [];

        try
        {
            // Read content into byte array for audit storage and file size calculation
            await using var memoryStream = new MemoryStream();
            await content.CopyToAsync(memoryStream, cancellationToken);
            csvContent = memoryStream.ToArray();
            fileSize = csvContent.Length;

            // Execute transfer with retry logic (3 attempts)
            var maxAttempts = 3;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    _logger.LogInformation(
                        "MOCK: Transferring file {FileName} ({FileSize} bytes) to {Destination} (attempt {Attempt}/{MaxAttempts})",
                        fileName, fileSize, destination, attempt, maxAttempts);

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

            var durationMs = (long)(DateTimeOffset.UtcNow - startTime).TotalMilliseconds;

            _logger.LogInformation(
                "File transfer completed successfully in {Duration}ms. File: {FileName}, Size: {FileSize} bytes",
                durationMs, fileName, fileSize);

            // Create FileTransferAudit entry for successful transfer
            await CreateFileTransferAuditAsync(
                checkRunWorkflowId,
                fileName,
                destination,
                csvContent,
                fileSize,
                isSuccess: true,
                errorMessage: null,
                durationMs,
                cancellationToken);

            // Create FtpOperationLog entry for successful transfer
            await CreateFtpOperationLogAsync(
                checkRunWorkflowId,
                FtpOperationType.Upload,
                fileName,
                destination,
                isSuccess: true,
                errorMessage: null,
                durationMs,
                cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            var durationMs = (long)(DateTimeOffset.UtcNow - startTime).TotalMilliseconds;

            _logger.LogError(ex,
                "File transfer failed after retries. File: {FileName}, Destination: {Destination}, Error: {Error}",
                fileName, destination, ex.Message);

            // Create FileTransferAudit entry for failed transfer
            await CreateFileTransferAuditAsync(
                checkRunWorkflowId,
                fileName,
                destination,
                csvContent,
                fileSize,
                isSuccess: false,
                errorMessage: ex.Message,
                durationMs,
                cancellationToken);

            // Create FtpOperationLog entry for failed transfer
            await CreateFtpOperationLogAsync(
                checkRunWorkflowId,
                FtpOperationType.Upload,
                fileName,
                destination,
                isSuccess: false,
                errorMessage: ex.Message,
                durationMs,
                cancellationToken);

            return Result<bool>.Failure(
                Error.FileTransferFailed with { Description = $"Failed to transfer file {fileName}: {ex.Message}" });
        }
    }

    /// <summary>
    /// Creates a FileTransferAudit entry in the database for detailed audit trail.
    /// </summary>
    private async Task CreateFileTransferAuditAsync(
        Guid? checkRunWorkflowId,
        string fileName,
        string destination,
        byte[] csvContent,
        long fileSize,
        bool isSuccess,
        string? errorMessage,
        long durationMs,
        CancellationToken cancellationToken)
    {
        try
        {
            await _contextFactory.UseWritableContext(async context =>
            {
                var audit = new FileTransferAudit
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTimeOffset.UtcNow,
                    FileName = fileName,
                    Destination = destination,
                    CsvContent = csvContent,
                    FileSize = fileSize,
                    IsSuccess = isSuccess,
                    ErrorMessage = errorMessage,
                    TransferDurationMs = durationMs,
                    CheckRunWorkflowId = checkRunWorkflowId,
                    UserName = null // Will be set from HTTP context in production
                };

                context.FileTransferAudits.Add(audit);
                await context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "FileTransferAudit created: File={FileName}, Success={IsSuccess}, Duration={Duration}ms, Size={FileSize} bytes",
                    fileName, isSuccess, durationMs, fileSize);

                return Result<bool>.Success(true);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the entire transfer if audit logging fails
            _logger.LogError(ex,
                "Failed to create FileTransferAudit entry for file {FileName}. Transfer completed but audit logging failed.",
                fileName);
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
                    UserName = null // Will be set from HTTP context in production
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
