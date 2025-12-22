using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Administration;

public sealed record CommentTypeDto
{
    public required byte Id { get; init; }

    [UnmaskSensitive]
    public required string Name { get; init; }

    /// <summary>
    /// Indicates whether this comment type is protected from name changes.
    /// Once set to true, can only be unset via direct database update.
    /// </summary>
    public bool IsProtected { get; init; }

    public DateTimeOffset? ModifiedAtUtc { get; init; }

    public string? UserName { get; init; }

    public static CommentTypeDto ResponseExample()
    {
        return new CommentTypeDto
        {
            Id = 1,
            Name = "Example Comment Type",
            IsProtected = false,
            ModifiedAtUtc = DateTimeOffset.UtcNow,
            UserName = "admin_user"
        };
    }
}
