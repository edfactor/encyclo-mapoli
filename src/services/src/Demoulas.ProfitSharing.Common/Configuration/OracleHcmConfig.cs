namespace Demoulas.ProfitSharing.Common.Configuration;
public sealed record OracleHcmConfig
{
    public required string Url { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string RestFrameworkVersion { get; set; } = "9";
    public byte Limit { get; set; } = 75;
    public string CronSchedule { get; set; } = "0 0 0 * * ?"; // Runs daily at midnight
}
