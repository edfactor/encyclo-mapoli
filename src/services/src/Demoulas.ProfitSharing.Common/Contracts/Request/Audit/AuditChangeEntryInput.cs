namespace Demoulas.ProfitSharing.Common.Contracts.Request.Audit;

public sealed record AuditChangeEntryInput
{
    public required string ColumnName { get; init; }

    public string? OriginalValue { get; init; }

    public string? NewValue { get; init; }

    public static AuditChangeEntryInput RequestExample()
    {
        return new AuditChangeEntryInput
        {
            ColumnName = "BadgeNumber",
            OriginalValue = "1001",
            NewValue = "1002"
        };
    }
}
