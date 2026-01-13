using Demoulas.ProfitSharing.Common.Contracts;

namespace Demoulas.ProfitSharing.Common.Interfaces.FileTransfer;

/// <summary>
/// Service for transferring files to external destinations (SFTP, FTP, etc.).
/// </summary>
public interface IFileTransferService
{
    /// <summary>
    /// Transfers a file to the specified destination with automatic retry logic.
    /// </summary>
    /// <param name="content">File content stream to transfer.</param>
    /// <param name="destination">Destination path (e.g., "/production/OutBox/VENUS/").</param>
    /// <param name="fileName">File name (e.g., "PPFIL.csv").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure with error details.</returns>
    Task<Result<bool>> TransferFileAsync(
        Stream content,
        string destination,
        string fileName,
        CancellationToken cancellationToken);
}
