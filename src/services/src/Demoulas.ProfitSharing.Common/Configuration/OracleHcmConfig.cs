namespace Demoulas.ProfitSharing.Common.Configuration;
public sealed record OracleHcmConfig
{
    public required string BaseAddress { get; set; }
    public required string DemographicUrl { get; set; }
    public string? PayrollUrl { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string RestFrameworkVersion { get; set; } = "9";
    public byte Limit { get; set; } = 75;
    public byte IntervalInHours { get; set; } = 24; // Runs every 24 hours
}
