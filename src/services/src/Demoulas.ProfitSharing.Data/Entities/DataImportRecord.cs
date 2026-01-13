namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class DataImportRecord
{
    public int Id { get; set; }
    public string SourceSchema { get; set; } = string.Empty;
    public DateTimeOffset ImportDateTimeUtc { get; init; } = DateTimeOffset.UtcNow;
}
