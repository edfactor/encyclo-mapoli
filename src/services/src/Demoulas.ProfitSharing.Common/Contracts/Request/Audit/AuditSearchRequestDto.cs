using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.Audit;

/// <summary>
/// Request DTO for searching audit events with filtering and pagination.
/// </summary>
public sealed record AuditSearchRequestDto : SortedPaginationRequestDto
{
    /// <summary>
    /// Filter by table name (uses LIKE matching).
    /// </summary>
    public string? TableName { get; init; }

    /// <summary>
    /// Filter by operation (uses LIKE matching).
    /// </summary>
    public string? Operation { get; init; }

    /// <summary>
    /// Filter by username (uses LIKE matching).
    /// </summary>
    public string? UserName { get; init; }

    /// <summary>
    /// Filter by events created on or after this time.
    /// </summary>
    public DateTimeOffset? StartTime { get; init; }

    /// <summary>
    /// Filter by events created before or at this time.
    /// </summary>
    public DateTimeOffset? EndTime { get; init; }
}
