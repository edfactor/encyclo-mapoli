namespace Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

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

    public static DemographicSyncAuditDto ResponseExample()
    {
        return new DemographicSyncAuditDto
        {
            Id = 1001,
            BadgeNumber = 12345,
            OracleHcmId = 567890,
            Message = "SSN mismatch detected during sync",
            PropertyName = "SocialSecurityNumber",
            InvalidValue = "XXX-XX-6789",
            UserName = "system_sync",
            Created = DateTimeOffset.UtcNow
        };
    }
}
