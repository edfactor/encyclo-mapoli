namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class DemographicSyncAudit
{
    public long Id { get; set; }
    public required int BadgeNumber { get; set; }
    public string? UserName { get; set; }
    public required string Message { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
}
