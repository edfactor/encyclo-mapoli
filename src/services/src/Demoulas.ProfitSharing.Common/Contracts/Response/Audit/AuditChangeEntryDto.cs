namespace Demoulas.ProfitSharing.Common.Contracts.Response.Audit;

/// <summary>
/// DTO for audit change entry information.
/// </summary>
public sealed record AuditChangeEntryDto
{
    /// <summary>
    /// The unique identifier for the audit change entry.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// The name of the column that was changed.
    /// </summary>
    public required string ColumnName { get; init; }

    /// <summary>
    /// The original value before the change as a JSON string.
    /// </summary>
    public string? OriginalValue { get; init; }

    /// <summary>
    /// The new value after the change as a JSON string.
    /// </summary>
    public string? NewValue { get; init; }
}
