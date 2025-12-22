using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Administration;

public sealed record CommentTypeDto
{
    public required byte Id { get; init; }

    [UnmaskSensitive]
    public required string Name { get; init; }

    public DateOnly? DateModified { get; init; }

    public string? UserModified { get; init; }

    public static CommentTypeDto ResponseExample()
    {
        return new CommentTypeDto
        {
            Id = 1,
            Name = "Example Comment Type",
            DateModified = DateOnly.FromDateTime(DateTime.Today),
            UserModified = "admin_user"
        };
    }
}
