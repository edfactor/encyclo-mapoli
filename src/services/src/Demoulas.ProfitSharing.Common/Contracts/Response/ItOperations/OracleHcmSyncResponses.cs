namespace Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

using Demoulas.ProfitSharing.Common.Attributes;

/// <summary>
/// Response containing OracleHcm sync metadata with four timestamp fields.
/// </summary>
[NoMemberDataExposed]
public class OracleHcmSyncMetadataResponse
{
    /// <summary>Most recent create timestamp from Demographic table (UTC).</summary>
    public DateTimeOffset? DemographicCreatedAtUtc { get; set; }

    /// <summary>Most recent modify timestamp from Demographic table (UTC).</summary>
    public DateTimeOffset? DemographicModifiedAtUtc { get; set; }

    /// <summary>Most recent create timestamp from PayProfit table (UTC).</summary>
    public DateTimeOffset? PayProfitCreatedAtUtc { get; set; }

    /// <summary>Most recent modify timestamp from PayProfit table (UTC).</summary>
    public DateTimeOffset? PayProfitModifiedAtUtc { get; set; }
}

/// <summary>
/// Response containing a single demographic sync audit record.
/// </summary>
[NoMemberDataExposed]
public class DemographicSyncAuditRecordResponse
{
    public long Id { get; set; }
    public int BadgeNumber { get; set; }
    public long OracleHcmId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? PropertyName { get; set; }
    public string? InvalidValue { get; set; }
    public string? UserName { get; set; }
    public DateTimeOffset Created { get; set; }
}

/// <summary>
/// Response indicating success of clearing demographic sync audit records.
/// </summary>
[NoMemberDataExposed]
public class ClearAuditResponse
{
    /// <summary>Number of records deleted.</summary>
    public int DeletedCount { get; set; }
}

/// <summary>
/// Internal DTO for OracleHcm sync metadata (four timestamp fields).
/// Used by service layer to return metadata to endpoints.
/// </summary>
public class OracleHcmSyncMetadata
{
    public DateTimeOffset? DemographicCreatedAtUtc { get; set; }
    public DateTimeOffset? DemographicModifiedAtUtc { get; set; }
    public DateTimeOffset? PayProfitCreatedAtUtc { get; set; }
    public DateTimeOffset? PayProfitModifiedAtUtc { get; set; }
}

/// <summary>
/// Internal DTO for a demographic sync audit record.
/// Used by service layer for mapping entity to DTO.
/// </summary>
public class DemographicSyncAuditDto
{
    public long Id { get; set; }
    public int BadgeNumber { get; set; }
    public long OracleHcmId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? PropertyName { get; set; }
    public string? InvalidValue { get; set; }
    public string? UserName { get; set; }
    public DateTimeOffset Created { get; set; }
}
