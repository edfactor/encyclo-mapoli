namespace Demoulas.ProfitSharing.Data.Entities.Audit;

public sealed class AuditEvent
{
    public long Id { get; set; }
    public required string? TableName { get; set; }
    public required string Operation { get; set; }
    public string? PrimaryKey { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public List<AuditChangeEntry>? ChangesJson { get; set; }
    public string? ChangesHash { get; set; }
}
