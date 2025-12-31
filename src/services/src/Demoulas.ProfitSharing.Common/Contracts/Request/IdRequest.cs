namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record IdRequest
{
    public required int Id { get; set; }

    public static IdRequest RequestExample() => new() { Id = 1001 };
}
