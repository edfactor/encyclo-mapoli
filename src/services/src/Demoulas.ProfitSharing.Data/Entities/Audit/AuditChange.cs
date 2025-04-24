namespace Demoulas.ProfitSharing.Data.Entities.Audit;

public sealed class AuditChange
{
    public long Id { get; set; }
    public required string ColumnName { get; set; }
    public string? OriginalValue { get; set; }
    public required string? NewValue { get; set; }
    public string UserName { get; set; } = null!;
    public DateTimeOffset ChangeDate { get; set; }
}
