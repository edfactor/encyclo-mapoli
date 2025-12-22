namespace Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

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

    public static OracleHcmSyncMetadata ResponseExample()
    {
        return new OracleHcmSyncMetadata
        {
            DemographicCreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-7),
            DemographicModifiedAtUtc = DateTimeOffset.UtcNow.AddDays(-1),
            PayProfitCreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-30),
            PayProfitModifiedAtUtc = DateTimeOffset.UtcNow.AddDays(-2)
        };
    }
}
