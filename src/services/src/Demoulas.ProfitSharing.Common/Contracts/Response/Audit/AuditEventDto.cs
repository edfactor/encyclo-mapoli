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

/// <summary>
/// DTO for audit event information returned in search results.
/// </summary>
public sealed record AuditEventDto
{
    /// <summary>
    /// The unique identifier for the audit event.
    /// </summary>
    public long AuditEventId { get; init; }

    /// <summary>
    /// The name of the database table that was audited.
    /// </summary>
    public required string? TableName { get; init; }

    /// <summary>
    /// The operation performed (e.g., Insert, Update, Delete, Archive).
    /// </summary>
    public required string Operation { get; init; }

    /// <summary>
    /// The primary key value of the affected record.
    /// </summary>
    public string? PrimaryKey { get; init; }

    /// <summary>
    /// The username of the person who performed the operation.
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// The timestamp when the audit event was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// The list of changes made to the record.
    /// Only populated when TableName is "NAVIGATION".
    /// </summary>
    public List<AuditChangeEntryDto>? ChangesJson { get; init; }
}
