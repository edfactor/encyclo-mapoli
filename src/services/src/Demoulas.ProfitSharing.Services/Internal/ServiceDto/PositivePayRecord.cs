namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

/// <summary>
/// Record representing a single row in the Positive Pay CSV file.
/// CsvHelper auto-maps properties by name. DateOnly automatically converts to string.
/// </summary>
public sealed class PositivePayRecord
{
    public int CheckNumber { get; init; }
    public decimal Amount { get; init; }
    public DateOnly IssueDate { get; init; }
    public string AccountNumber { get; init; } = string.Empty;
    public int BadgeNumber { get; init; }
}
