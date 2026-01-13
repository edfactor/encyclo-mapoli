namespace Demoulas.ProfitSharing.Common.Contracts.Messaging;

public sealed record MessageRequest<TMessageBody> where TMessageBody : class
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string UserId { get; init; }
    public byte Version { get; set; } = 1;
    public required string ApplicationName { get; set; }
    public DateTimeOffset GeneratedUtc { get; set; } = DateTimeOffset.UtcNow;

    public required TMessageBody Body { get; set; }
}
