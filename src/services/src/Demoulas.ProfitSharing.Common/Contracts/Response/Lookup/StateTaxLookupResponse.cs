using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
[NoMemberDataExposed]
public sealed record StateTaxLookupResponse
{
    public required string State { get; init; }
    public required decimal StateTaxRate { get; init; }
}
