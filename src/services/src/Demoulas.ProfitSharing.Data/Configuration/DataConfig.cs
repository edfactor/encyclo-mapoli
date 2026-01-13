namespace Demoulas.ProfitSharing.Data.Configuration;

public sealed record DataConfig
{
    public bool EnableAudit { get; set; } = true;
}
