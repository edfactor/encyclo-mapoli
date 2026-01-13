namespace Demoulas.ProfitSharing.Common.Contracts.Response.CheckRun;

/// <summary>
/// Represents a generated check-run print file.
/// </summary>
public sealed record CheckRunPrintFileResult
{
    public required Guid RunId { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required string Content { get; init; }
    public required int CheckCount { get; init; }
}
