namespace Demoulas.ProfitSharing.Common.Contracts.Response.Audit;

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

    public static AuditEventDto ResponseExample()
    {
        return new AuditEventDto
        {
            AuditEventId = 1234,
            TableName = "NAVIGATION",
            Operation = "Update",
            PrimaryKey = "5",
            UserName = "admin@example.com",
            CreatedAt = DateTimeOffset.UtcNow,
            ChangesJson = new List<AuditChangeEntryDto>
            {
                AuditChangeEntryDto.ResponseExample()
            }
        };
    }
}
