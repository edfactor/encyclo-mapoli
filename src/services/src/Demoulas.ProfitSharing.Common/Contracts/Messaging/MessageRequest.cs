namespace Demoulas.ProfitSharing.Common.Contracts.Messaging;
public sealed record MessageRequest<TMessageBody> where TMessageBody : class
{
    public byte Version { get; set; } = 1;
    public required string ApplicationName { get; set; }
    public DateTimeOffset Generated { get; set; } = DateTimeOffset.UtcNow;

    public required TMessageBody Body { get; set; }
}
