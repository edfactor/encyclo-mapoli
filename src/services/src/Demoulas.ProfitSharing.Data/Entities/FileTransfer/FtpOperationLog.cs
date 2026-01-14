namespace Demoulas.ProfitSharing.Data.Entities.FileTransfer;

/// <summary>
/// Audit log for FTP operations performed during check run workflow.
/// Tracks file transfers for compliance, debugging, and operational monitoring.
/// </summary>
public sealed class FtpOperationLog
{
    /// <summary>
    /// Unique identifier for this log entry.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the check run workflow this operation is associated with.
    /// </summary>
    public Guid CheckRunWorkflowId { get; set; }

    /// <summary>
    /// Type of FTP operation performed (Upload, Download, Delete).
    /// </summary>
    public FtpOperationType OperationType { get; set; }

    /// <summary>
    /// Name of the file being transferred (e.g., "MICR_2024_12_25.txt").
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Destination path or server where file was transferred.
    /// </summary>
    public string Destination { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the operation completed successfully.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Error message if operation failed, null if successful.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Duration of the operation in milliseconds.
    /// </summary>
    public long DurationMs { get; set; }

    /// <summary>
    /// Timestamp when the operation was performed.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// User who initiated the operation.
    /// </summary>
    public string? UserName { get; set; }
}
