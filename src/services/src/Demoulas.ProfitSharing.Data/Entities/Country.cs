using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class Country : LookupTable<byte>
{
    public required string ISO { get; init; }
    public required string TelephoneCode { get; init; }
}
