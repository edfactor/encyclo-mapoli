using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

[NoMemberDataExposed]
public sealed record IdsResponse
{
    public required int[] Ids { get; set; }

    public static IdsResponse ResponseExample() => new IdsResponse
    {
        Ids = new[] { 1, 2, 3 }
    };
}
