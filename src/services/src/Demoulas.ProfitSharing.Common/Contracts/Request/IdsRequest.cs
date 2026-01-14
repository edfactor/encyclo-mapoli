namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record IdsRequest
{
    public int[] Ids { get; init; } = Array.Empty<int>();

    public static IdsRequest RequestExample() => new IdsRequest
    {
        Ids = new[] { 1, 2, 3 }
    };
}
