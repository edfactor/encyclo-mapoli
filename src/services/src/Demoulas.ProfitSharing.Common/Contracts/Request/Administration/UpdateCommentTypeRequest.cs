namespace Demoulas.ProfitSharing.Common.Contracts.Request.Administration;

public sealed record UpdateCommentTypeRequest
{
    public required byte Id { get; init; }
    public required string Name { get; init; }
}
