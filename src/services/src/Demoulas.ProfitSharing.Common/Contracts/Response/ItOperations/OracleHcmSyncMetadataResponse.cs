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
