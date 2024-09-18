namespace Demoulas.ProfitSharing.Common.Configuration;
public sealed record OracleHcmConfig
{
    public required string Url { get; set; } = "http://localhost/noservice";
    public string? Username { get; set; } = "something";
    public string? Password { get; set; } = "something pw";
    public string RestFrameworkVersion { get; set; } = "9";
    public byte Limit { get; set; } = 125;
}
