namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// Audit record for file transfer operations to external systems.
/// Provides complete audit trail for compliance and replay capability.
/// </summary>
public class FileTransferAudit
{
    public Guid Id { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public required string FileName { get; set; }
    public required string Destination { get; set; }
    public long FileSize { get; set; }
    public long TransferDurationMs { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? UserName { get; set; }

    /// <summary>
    /// Stored file content for replay capability if transfer fails.
    /// </summary>
    public byte[]? CsvContent { get; set; }

    /// <summary>
    /// Optional link to CheckRunWorkflow if this transfer is part of a workflow.
    /// </summary>
    public Guid? CheckRunWorkflowId { get; set; }
}
