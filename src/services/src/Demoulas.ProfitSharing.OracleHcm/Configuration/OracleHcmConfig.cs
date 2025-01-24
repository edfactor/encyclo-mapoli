namespace Demoulas.ProfitSharing.OracleHcm.Configuration;

/// <summary>
/// Represents the configuration settings for the Oracle HCM integration.
/// Provides properties for enabling sync operations, API endpoints, credentials, and sync parameters.
/// </summary>
public sealed record OracleHcmConfig
{
    public bool EnableSync { get; set; } = true;
    public required string BaseAddress { get; set; }
    public required string DemographicUrl { get; set; }
    public string? PayrollUrl { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string RestFrameworkVersion { get; set; } = "9";
    public byte Limit { get; set; } = 75;
    public byte IntervalInHours { get; set; } = 150;
    public byte PayrollIntervalInHours { get; set; } = 150;

    /// <summary>
    /// Gets or sets the interval in minutes for synchronizing delta changes in employee data.
    /// This property is used to configure the frequency of delta synchronization operations
    /// within the Oracle HCM integration.
    /// </summary>
    public byte DeltaIntervalInMinutes { get; set; } = 15;
}
