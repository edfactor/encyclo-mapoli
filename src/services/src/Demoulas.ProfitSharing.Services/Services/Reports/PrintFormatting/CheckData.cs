namespace Demoulas.ProfitSharing.Services.Services.Reports.PrintFormatting;

/// <summary>
/// Represents data required for check printing.
/// </summary>
public sealed class CheckData
{
    public required int CheckNumber { get; init; }
    public required decimal Amount { get; init; }
    public required string RecipientName { get; init; }
    public required string Ssn { get; init; }
    public required int BadgeNumber { get; init; }
    public required DateOnly IssueDate { get; init; }
}
