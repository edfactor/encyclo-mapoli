using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class Country : ILookupTable<short>
{
    public short Id { get; set; }
    public required string Name { get; set; }

    public required string ISO { get; init; }
    public required string TelephoneCode { get; init; }
}
