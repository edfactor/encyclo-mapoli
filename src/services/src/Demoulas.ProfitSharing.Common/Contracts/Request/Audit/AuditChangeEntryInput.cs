namespace Demoulas.ProfitSharing.Common.Contracts.Request.Audit;

public sealed record AuditChangeEntryInput
{
    public required string ColumnName { get; init; }

    public string? OriginalValue { get; init; }

    public string? NewValue { get; init; }
}
