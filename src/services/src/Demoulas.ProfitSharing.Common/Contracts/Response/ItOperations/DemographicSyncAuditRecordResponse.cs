using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

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

    public static DemographicSyncAuditRecordResponse ResponseExample()
    {
        return new DemographicSyncAuditRecordResponse
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
