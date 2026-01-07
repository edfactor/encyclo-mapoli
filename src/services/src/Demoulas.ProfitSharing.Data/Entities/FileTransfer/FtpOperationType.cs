namespace Demoulas.ProfitSharing.Data.Entities.FileTransfer;

/// <summary>
/// Represents the type of FTP operation performed during file transfer.
/// </summary>
public enum FtpOperationType
{
    /// <summary>
    /// File was uploaded to remote server.
    /// </summary>
    Upload = 0,

    /// <summary>
    /// File was downloaded from remote server.
    /// </summary>
    Download = 1,

    /// <summary>
    /// File was deleted from remote server.
    /// </summary>
    Delete = 2
}
