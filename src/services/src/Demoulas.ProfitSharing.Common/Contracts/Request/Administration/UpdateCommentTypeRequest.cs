namespace Demoulas.ProfitSharing.Common.Contracts.Request.Administration;

public sealed record UpdateCommentTypeRequest
{
    public required byte Id { get; init; }
    public required string Name { get; init; }

    /// <summary>
    /// Indicates whether this comment type is protected from name changes.
    /// Can be set from false to true, but cannot be set from true to false (one-way protection).
    /// </summary>
    public bool IsProtected { get; init; }
}
