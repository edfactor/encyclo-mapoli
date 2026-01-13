namespace Demoulas.ProfitSharing.Common.Contracts.Request.Administration;

public sealed record CreateCommentTypeRequest
{
    public required string Name { get; init; }

    /// <summary>
    /// Indicates whether this comment type is protected from name changes.
    /// Defaults to false if not specified.
    /// </summary>
    public bool IsProtected { get; init; }

    public static CreateCommentTypeRequest RequestExample() => new()
    {
        Name = "New Comment Type",
        IsProtected = false
    };
}
