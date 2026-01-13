namespace Demoulas.ProfitSharing.Data.Entities.Audit;

public sealed class AuditChangeEntry
{
    public long Id { get; set; }
    public required string ColumnName { get; set; }
    public string? OriginalValue { get; set; }
    public required string? NewValue { get; set; }
}
