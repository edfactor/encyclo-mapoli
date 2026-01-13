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

    public static OracleHcmSyncMetadataResponse ResponseExample()
    {
        return new OracleHcmSyncMetadataResponse
        {
            DemographicCreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-7),
            DemographicModifiedAtUtc = DateTimeOffset.UtcNow.AddDays(-1),
            PayProfitCreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-30),
            PayProfitModifiedAtUtc = DateTimeOffset.UtcNow.AddDays(-2)
        };
    }
}
