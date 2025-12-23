using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.FileTransfer;

/// <summary>
/// Mock implementation of file transfer service for development and testing.
/// Uses .NET built-in retry logic for resilience.
/// Production implementation will use SFTP client to transfer to /production/OutBox/VENUS/.
/// </summary>
public sealed class MockFileTransferService : IFileTransferService
{
    private readonly ILogger<MockFileTransferService> _logger;

    public MockFileTransferService(ILogger<MockFileTransferService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Simulates file transfer with retry logic and comprehensive logging.
    /// In production, this will use SFTP client to transfer to /production/OutBox/VENUS/.
    /// </summary>
    public async Task<Result<FileTransferAudit>> TransferFileAsync(
        Stream content,
        string destination,
        string fileName,
        CancellationToken cancellationToken)
    {
        var startTime = DateTimeOffset.UtcNow;
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
                "File transfer completed successfully. File: {FileName}, Size: {FileSize} bytes, Duration: {Duration}ms",
                fileName, audit.FileSize, audit.TransferDurationMs);

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

            return Result<FileTransferAudit>.Failure(
                Error.FileTransferFailed with { Description = $"Failed to transfer file {fileName}: {ex.Message}" });
        }
    }
}
